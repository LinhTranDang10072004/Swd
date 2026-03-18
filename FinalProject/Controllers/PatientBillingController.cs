using System.Security.Claims;
using FinalProject.Auth;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Patient)]
public class PatientBillingController : Controller
{
    private readonly HospitalManagementContext _db;

    public PatientBillingController(HospitalManagementContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Invoices()
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        var invoices = await _db.Invoices.AsNoTracking()
            .Where(i => i.PatientId == patientId.Value)
            .Include(i => i.Appointment)!.ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.IssuedAt)
            .Take(200)
            .ToListAsync();

        return View(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> Invoice(int id)
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        var invoice = await _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceId == id && i.PatientId == patientId.Value)
            .Include(i => i.Patient)
            .Include(i => i.Appointment)!.ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync();

        if (invoice is null)
        {
            return NotFound();
        }

        return View(invoice);
    }

    [HttpGet]
    public async Task<IActionResult> Payments()
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        var payments = await _db.Payments.AsNoTracking()
            .Where(p => p.Invoice.PatientId == patientId.Value)
            .Include(p => p.Invoice)
            .OrderByDescending(p => p.PaidAt)
            .Take(200)
            .ToListAsync();

        return View(payments);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadInvoice(int id)
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        var invoice = await _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceId == id && i.PatientId == patientId.Value)
            .Include(i => i.Patient)
            .Include(i => i.Appointment)!.ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync();

        if (invoice is null)
        {
            return NotFound();
        }

        var html = await RenderInvoiceHtml(invoice);
        var bytes = System.Text.Encoding.UTF8.GetBytes(html);
        return File(bytes, "text/html; charset=utf-8", $"invoice_{invoice.InvoiceId}.html");
    }

    private async Task<int?> GetCurrentPatientId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return null;
        }

        return await _db.Patients.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => (int?)p.PatientId)
            .FirstOrDefaultAsync();
    }

    private static Task<string> RenderInvoiceHtml(Invoice invoice)
    {
        var paidTotal = invoice.Payments.Where(p => p.Status == "SUCCESS").Sum(p => p.AmountPaid);
        var isPaid = string.Equals(invoice.Status, "PAID", StringComparison.OrdinalIgnoreCase) || paidTotal >= invoice.FinalAmount;
        var doctorName = invoice.Appointment?.Doctor?.User?.FullName ?? "";
        var apptTime = invoice.Appointment?.AppointmentDate.ToString("dd/MM/yyyy HH:mm") ?? "";

        var html = $@"
<!doctype html>
<html>
<head>
  <meta charset=""utf-8"" />
  <title>Invoice #{invoice.InvoiceId}</title>
  <style>
    body {{ font-family: Arial, sans-serif; margin: 24px; }}
    .muted {{ color: #666; }}
    table {{ border-collapse: collapse; width: 100%; margin-top: 16px; }}
    th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
    th {{ background: #f5f5f5; }}
    .right {{ text-align: right; }}
    @media print {{ .no-print {{ display: none; }} body {{ margin: 0; }} }}
  </style>
</head>
<body>
  <div class=""no-print"" style=""margin-bottom:12px;"">
    <button onclick=""window.print()"">In hóa đơn</button>
  </div>
  <h2>Hóa đơn #{invoice.InvoiceId}</h2>
  <div class=""muted"">Ngày xuất: {invoice.IssuedAt:dd/MM/yyyy HH:mm}</div>
  <div style=""margin-top:12px;"">
    <div><b>Bệnh nhân</b>: {System.Net.WebUtility.HtmlEncode(invoice.Patient.FullName)}</div>
    <div><b>Trạng thái</b>: {(isPaid ? "PAID" : invoice.Status)}</div>
    <div><b>Lịch hẹn</b>: {System.Net.WebUtility.HtmlEncode(doctorName)} - {apptTime}</div>
  </div>

  <table>
    <tr><th>Tổng tiền</th><td class=""right"">{invoice.TotalAmount:N0}</td></tr>
    <tr><th>Giảm giá</th><td class=""right"">{(invoice.Discount ?? 0):N0}</td></tr>
    <tr><th>Thành tiền</th><td class=""right"">{invoice.FinalAmount:N0}</td></tr>
    <tr><th>Đã thanh toán</th><td class=""right"">{paidTotal:N0}</td></tr>
  </table>

  <h3>Chi tiết thanh toán</h3>
  <table>
    <thead>
      <tr><th>Thời gian</th><th>Phương thức</th><th>Trạng thái</th><th class=""right"">Số tiền</th></tr>
    </thead>
    <tbody>
      {(invoice.Payments.Count == 0 ? "<tr><td colspan=\"4\" class=\"muted\">Chưa có giao dịch thanh toán.</td></tr>" :
        string.Join("", invoice.Payments.OrderByDescending(p => p.PaidAt).Select(p =>
          $"<tr><td>{p.PaidAt:dd/MM/yyyy HH:mm}</td><td>{System.Net.WebUtility.HtmlEncode(p.PaymentMethod)}</td><td>{System.Net.WebUtility.HtmlEncode(p.Status)}</td><td class=\"right\">{p.AmountPaid:N0}</td></tr>"
        )))}
    </tbody>
  </table>
</body>
</html>";

        return Task.FromResult(html);
    }
}

