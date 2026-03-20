using FinalProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Services;

public class InvoiceDetailService : IInvoiceDetailService
{
    private readonly HospitalManagementContext _db;

    public InvoiceDetailService(HospitalManagementContext db)
    {
        _db = db;
    }

    public async Task<int?> GetInvoiceOwnerPatientIdAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceId == invoiceId)
            .Select(i => (int?)i.PatientId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Invoice?> GetDetailedBreakdownAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceId == invoiceId)
            .Include(i => i.Patient)
            .Include(i => i.Appointment)!.ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Invoice>> GetInvoicesForPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _db.Invoices.AsNoTracking()
            .Where(i => i.PatientId == patientId)
            .Include(i => i.Appointment)!.ThenInclude(a => a!.Doctor).ThenInclude(d => d!.User)
            .Include(i => i.Payments)
            .OrderByDescending(i => i.IssuedAt)
            .Take(200)
            .ToListAsync(cancellationToken);
    }
}

