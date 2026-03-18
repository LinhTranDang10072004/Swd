using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class PurchaseOrder
{
    public int OrderId { get; set; }

    public int SupplierId { get; set; }

    public int RequestedBy { get; set; }

    public DateTime OrderDate { get; set; }

    public DateOnly? ExpectedDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual ICollection<ApprovalAction> ApprovalActions { get; set; } = new List<ApprovalAction>();

    public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; } = new List<GoodsReceiptNote>();

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual Pharmacist RequestedByNavigation { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
