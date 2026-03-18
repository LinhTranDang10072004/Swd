using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? SupplierCode { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? TaxCode { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
