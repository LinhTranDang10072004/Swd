using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Pharmacist
{
    public int PharmacistId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<AuditSession> AuditSessions { get; set; } = new List<AuditSession>();

    public virtual ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; } = new List<GoodsReceiptNote>();

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
