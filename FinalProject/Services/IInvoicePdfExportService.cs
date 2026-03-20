using FinalProject.Models;

namespace FinalProject.Services;

public interface IInvoicePdfExportService
{
    Task<InvoiceExportFile> GenerateInvoicePDFAsync(Invoice invoiceDetail, CancellationToken cancellationToken = default);
}

public sealed record InvoiceExportFile(byte[] Content, string ContentType, string FileName);

