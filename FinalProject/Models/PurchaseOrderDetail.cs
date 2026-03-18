using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class PurchaseOrderDetail
{
    public int DetailId { get; set; }

    public int OrderId { get; set; }

    public int MedicineId { get; set; }

    public int QuantityOrdered { get; set; }

    public int? QuantityReceived { get; set; }

    public decimal? UnitPrice { get; set; }

    public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetail>();

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual PurchaseOrder Order { get; set; } = null!;
}
