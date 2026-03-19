using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels;

public class DispenseLookupViewModel
{
    [Required]
    [Display(Name = "Mã đơn thuốc (Prescription ID)")]
    public int? PrescriptionId { get; set; }
}

public class DispenseReviewViewModel
{
    public int PrescriptionId { get; set; }
    public string PrescriptionStatus { get; set; } = "";

    public string? PatientName { get; set; }
    public DateTime IssuedAt { get; set; }

    public List<DispenseReviewItemViewModel> Items { get; set; } = new();
}

public class DispenseReviewItemViewModel
{
    public int PrescriptionDetailId { get; set; }
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = "";

    [Display(Name = "Số lượng kê")]
    public int PrescribedQty { get; set; }

    [Display(Name = "Tồn kho khả dụng")]
    public int AvailableQty { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    [Display(Name = "Số lượng phát")]
    public int DispenseQty { get; set; }
}

