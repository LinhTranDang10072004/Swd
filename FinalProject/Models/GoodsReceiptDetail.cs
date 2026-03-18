using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class GoodsReceiptDetail
{
    public int DetailId { get; set; }

    public int GrnId { get; set; }

    public int? PurchaseOrderDetailId { get; set; }

    public int MedicineId { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ManufactureDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int QuantityReceived { get; set; }

    public decimal? UnitCost { get; set; }

    public int? InventoryId { get; set; }

    public virtual GoodsReceiptNote Grn { get; set; } = null!;

    public virtual Inventory? Inventory { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual PurchaseOrderDetail? PurchaseOrderDetail { get; set; }
}
