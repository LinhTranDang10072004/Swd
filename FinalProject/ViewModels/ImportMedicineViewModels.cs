using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class ImportLookupViewModel
{
    [Required]
    [Display(Name = "Mã đơn mua (PO ID)")]
    public int? OrderId { get; set; }
}

public class ImportReceiveViewModel
{
    public int OrderId { get; set; }
    public string OrderStatus { get; set; } = "";
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = "";

    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    public List<ImportReceiveItemViewModel> Items { get; set; } = new();
}

public class ImportReceiveItemViewModel
{
    public int PurchaseOrderDetailId { get; set; }
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = "";

    [Display(Name = "Số lượng đặt")]
    public int OrderedQty { get; set; }

    [Display(Name = "Đã nhận trước đó")]
    public int AlreadyReceivedQty { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    [Display(Name = "Số lượng nhận lần này")]
    public int ReceiveQty { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Số lô (Batch)")]
    public string BatchNumber { get; set; } = "";

    [Display(Name = "NSX")]
    public DateOnly? ManufactureDate { get; set; }

    [Required]
    [Display(Name = "HSD")]
    public DateOnly? ExpirationDate { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Giá vốn / đơn vị")]
    public decimal? UnitCost { get; set; }

    [Range(0, 999999999)]
    [Display(Name = "Giá bán / đơn vị")]
    public decimal? SellingPrice { get; set; }
}

