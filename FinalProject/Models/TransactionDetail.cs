using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class TransactionDetail
{
    public int DetailId { get; set; }

    public int TransactionId { get; set; }

    public int PrescriptionDetailId { get; set; }

    public int InventoryId { get; set; }

    public int QuantityDispensed { get; set; }

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual PrescriptionDetail PrescriptionDetail { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
