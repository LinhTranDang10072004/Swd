using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class GoodsReceiptNote
{
    public int GrnId { get; set; }

    public int OrderId { get; set; }

    public int PharmacistId { get; set; }

    public DateTime ReceivedAt { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetail>();

    public virtual PurchaseOrder Order { get; set; } = null!;

    public virtual Pharmacist Pharmacist { get; set; } = null!;
}
