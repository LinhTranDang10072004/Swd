using System.Text;
using FinalProject.Models;

namespace FinalProject.Services;

public class InvoicePdfExportService : IInvoicePdfExportService
{
    public Task<InvoiceExportFile> GenerateInvoicePDFAsync(Invoice invoiceDetail, CancellationToken cancellationToken = default)
    {
        // Note: Current implementation exports an HTML invoice for printing/download.
        // This keeps the service boundary (PDFExportService) consistent with your design.
        var paidTotal = invoiceDetail.Payments.Where(p => p.Status == "SUCCESS").Sum(p => p.AmountPaid);
        var isPaid = string.Equals(invoiceDetail.Status, "PAID", StringComparison.OrdinalIgnoreCase) || paidTotal >= invoiceDetail.FinalAmount;

        var doctorName = invoiceDetail.Appointment?.Doctor?.User?.FullName ?? "";
        var apptTime = invoiceDetail.Appointment?.AppointmentDate.ToString("dd/MM/yyyy HH:mm") ?? "";

        var html = $@"
<!doctype html>
<html>
<head>
  <meta charset=""utf-8"" />
  <title>Invoice #{invoiceDetail.InvoiceId}</title>
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
  <h2>Hóa đơn #{invoiceDetail.InvoiceId}</h2>
  <div class=""muted"">Ngày xuất: {invoiceDetail.IssuedAt:dd/MM/yyyy HH:mm}</div>
  <div style=""margin-top:12px;"">
    <div><b>Bệnh nhân</b>: {System.Net.WebUtility.HtmlEncode(invoiceDetail.Patient.FullName)}</div>
    <div><b>Trạng thái</b>: {(isPaid ? "PAID" : invoiceDetail.Status)}</div>
    <div><b>Lịch hẹn</b>: {System.Net.WebUtility.HtmlEncode(doctorName)} - {apptTime}</div>
  </div>

  <table>
    <tr><th>Tổng tiền</th><td class=""right"">{invoiceDetail.TotalAmount:N0}</td></tr>
    <tr><th>Giảm giá</th><td class=""right"">{(invoiceDetail.Discount ?? 0):N0}</td></tr>
    <tr><th>Thành tiền</th><td class=""right"">{invoiceDetail.FinalAmount:N0}</td></tr>
    <tr><th>Đã thanh toán</th><td class=""right"">{paidTotal:N0}</td></tr>
  </table>

  <h3>Chi tiết thanh toán</h3>
  <table>
    <thead>
      <tr><th>Thời gian</th><th>Phương thức</th><th>Trạng thái</th><th class=""right"">Số tiền</th></tr>
    </thead>
    <tbody>
      {(invoiceDetail.Payments.Count == 0 ? "<tr><td colspan=\"4\" class=\"muted\">Chưa có giao dịch thanh toán.</td></tr>" :
        string.Join("", invoiceDetail.Payments.OrderByDescending(p => p.PaidAt).Select(p =>
          $@"<tr><td>{p.PaidAt:dd/MM/yyyy HH:mm}</td><td>{System.Net.WebUtility.HtmlEncode(p.PaymentMethod)}</td><td>{System.Net.WebUtility.HtmlEncode(p.Status)}</td><td class=""right"">{p.AmountPaid:N0}</td></tr>"
        )))}
    </tbody>
  </table>
</body>
</html>";

        var bytes = Encoding.UTF8.GetBytes(html);
        var contentType = "text/html; charset=utf-8";
        var fileName = $"invoice_{invoiceDetail.InvoiceId}.html";
        return Task.FromResult(new InvoiceExportFile(bytes, contentType, fileName));
    }
}

