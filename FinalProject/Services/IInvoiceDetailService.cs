using FinalProject.Models;

namespace FinalProject.Services;

public interface IInvoiceDetailService
{
    Task<int?> GetInvoiceOwnerPatientIdAsync(int invoiceId, CancellationToken cancellationToken = default);

    Task<Invoice?> GetDetailedBreakdownAsync(int invoiceId, CancellationToken cancellationToken = default);

    Task<List<Invoice>> GetInvoicesForPatientAsync(int patientId, CancellationToken cancellationToken = default);
}

