using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string MedicineName { get; set; } = null!;

    public string? GenericName { get; set; }

    public string? MedicineCode { get; set; }

    public string? Category { get; set; }

    public string? Unit { get; set; }

    public string? DosageForm { get; set; }

    public string? Description { get; set; }

    public bool? RequiresPrescription { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
}
