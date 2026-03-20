using System.Security.Claims;
using FinalProject.Auth;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject.Services;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Patient)]
public class PatientBillingController : Controller
{
    private readonly HospitalManagementContext _db;
    private readonly IInvoiceDetailService _invoiceDetailService;
    private readonly IInvoicePdfExportService _invoicePdfExportService;

    public PatientBillingController(
        HospitalManagementContext db,
        IInvoiceDetailService invoiceDetailService,
        IInvoicePdfExportService invoicePdfExportService)
    {
        _db = db;
        _invoiceDetailService = invoiceDetailService;
        _invoicePdfExportService = invoicePdfExportService;
    }

    [HttpGet]
    public async Task<IActionResult> Invoices(string? filterStatus = "ALL")
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        ViewData["FilterStatus"] = filterStatus ?? "ALL";

        var invoices = await _invoiceDetailService.GetInvoicesForPatientAsync(patientId.Value);

        IEnumerable<Invoice> filtered = invoices;
        if (string.Equals(filterStatus, "PAID", StringComparison.OrdinalIgnoreCase))
        {
            filtered = invoices.Where(IsInvoicePaid);
        }
        else if (string.Equals(filterStatus, "UNPAID", StringComparison.OrdinalIgnoreCase))
        {
            filtered = invoices.Where(inv => !IsInvoicePaid(inv));
        }

        return View(filtered.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Invoice(int id)
    {
        var patientId = await GetCurrentPatientId();
        if (patientId is null)
        {
            return Forbid();
        }

        // Security & privacy NFR (broken access control prevention):
        // The controller checks that the invoice belongs to the logged-in patient.
        var ownerPatientId = await _invoiceDetailService.GetInvoiceOwnerPatientIdAsync(id);
        if (ownerPatientId is null)
        {
            return NotFound();
        }

        if (ownerPatientId.Value != patientId.Value)
        {
            return Forbid();
        }

        var invoice = await _invoiceDetailService.GetDetailedBreakdownAsync(id);

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

        var ownerPatientId = await _invoiceDetailService.GetInvoiceOwnerPatientIdAsync(id);
        if (ownerPatientId is null)
        {
            return NotFound();
        }

        if (ownerPatientId.Value != patientId.Value)
        {
            return Forbid();
        }

        var invoice = await _invoiceDetailService.GetDetailedBreakdownAsync(id);

        if (invoice is null)
        {
            return NotFound();
        }

        var export = await _invoicePdfExportService.GenerateInvoicePDFAsync(invoice);
        return File(export.Content, export.ContentType, export.FileName);
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

    private static bool IsInvoicePaid(Invoice invoice)
    {
        var paidTotal = invoice.Payments.Where(p => p.Status == "SUCCESS").Sum(p => p.AmountPaid);
        return string.Equals(invoice.Status, "PAID", StringComparison.OrdinalIgnoreCase) || paidTotal >= invoice.FinalAmount;
    }
}

