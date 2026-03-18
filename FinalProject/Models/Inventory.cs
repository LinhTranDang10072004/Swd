using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int MedicineId { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ManufactureDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int StockQuantity { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? SellingPrice { get; set; }

    public string? Location { get; set; }

    public DateTime LastUpdated { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AuditDetail> AuditDetails { get; set; } = new List<AuditDetail>();

    public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetail>();

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
}
