using System.Security.Claims;
using FinalProject.Auth;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Pharmacist)]
public class DispenseController : Controller
{
    private const string ReadyToDispenseStatus = "READY_TO_DISPENSE";
    private const string DispensedStatus = "DISPENSED";
    private const string PartiallyDispensedStatus = "PARTIALLY_DISPENSED";

    private readonly HospitalManagementContext _db;

    public DispenseController(HospitalManagementContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new DispenseLookupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(DispenseLookupViewModel vm)
    {
        if (!ModelState.IsValid || vm.PrescriptionId is null)
        {
            return View(vm);
        }

        return RedirectToAction(nameof(Review), new { id = vm.PrescriptionId.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Review(int id)
    {
        var prescription = await _db.Prescriptions
            .AsNoTracking()
            .Include(p => p.Record)
            .ThenInclude(r => r.Patient)
            .Include(p => p.PrescriptionDetails)
            .ThenInclude(d => d.Medicine)
            .FirstOrDefaultAsync(p => p.PrescriptionId == id);

        if (prescription is null)
        {
            return NotFound("Prescription not found");
        }

        var medicineIds = prescription.PrescriptionDetails.Select(d => d.MedicineId).Distinct().ToList();
        var availableByMedicine = await _db.Inventories
            .AsNoTracking()
            .Where(i => medicineIds.Contains(i.MedicineId) && i.Status == "ACTIVE" && i.StockQuantity > 0)
            .GroupBy(i => i.MedicineId)
            .Select(g => new { MedicineId = g.Key, Qty = g.Sum(x => x.StockQuantity) })
            .ToDictionaryAsync(x => x.MedicineId, x => x.Qty);

        var vm = new DispenseReviewViewModel
        {
            PrescriptionId = prescription.PrescriptionId,
            PrescriptionStatus = prescription.Status,
            PatientName = prescription.Record?.Patient?.FullName,
            IssuedAt = prescription.IssuedAt,
            Items = prescription.PrescriptionDetails
                .Select(d => new DispenseReviewItemViewModel
                {
                    PrescriptionDetailId = d.DetailId,
                    MedicineId = d.MedicineId,
                    MedicineName = d.Medicine.MedicineName,
                    PrescribedQty = d.Quantity,
                    AvailableQty = availableByMedicine.TryGetValue(d.MedicineId, out var q) ? q : 0,
                    DispenseQty = Math.Min(d.Quantity, availableByMedicine.TryGetValue(d.MedicineId, out var q2) ? q2 : 0)
                })
                .ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(DispenseReviewViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Review), vm);
        }

        var pharmacistId = await GetCurrentPharmacistId();
        if (pharmacistId is null)
        {
            return Forbid();
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var prescription = await _db.Prescriptions
                .Include(p => p.PrescriptionDetails)
                .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(p => p.PrescriptionId == vm.PrescriptionId);

            if (prescription is null)
            {
                return NotFound("Prescription not found");
            }

            if (!string.Equals((prescription.Status ?? "").Trim(), ReadyToDispenseStatus, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Prescription is not cleared for dispensing.");
            }

            if (await _db.Transactions.AnyAsync(t => t.PrescriptionId == prescription.PrescriptionId))
            {
                return BadRequest("Prescription already dispensed.");
            }

            var requested = vm.Items.ToDictionary(x => x.PrescriptionDetailId, x => x.DispenseQty);
            var isPartial = false;

            var transaction = new Transaction
            {
                PrescriptionId = prescription.PrescriptionId,
                PharmacistId = pharmacistId.Value,
                Status = DispensedStatus,
                Notes = null
            };
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(); // get TransactionId

            foreach (var detail in prescription.PrescriptionDetails)
            {
                if (!requested.TryGetValue(detail.DetailId, out var dispenseQty))
                {
                    dispenseQty = 0;
                }

                if (dispenseQty < 0)
                {
                    throw new InvalidOperationException("Invalid dispense quantity.");
                }

                if (dispenseQty > detail.Quantity)
                {
                    throw new InvalidOperationException("Dispense quantity cannot exceed prescribed quantity.");
                }

                if (dispenseQty < detail.Quantity)
                {
                    isPartial = true;
                }

                if (dispenseQty == 0)
                {
                    continue;
                }

                // FIFO: oldest expiry/manufacture first
                var remaining = dispenseQty;
                var batches = await _db.Inventories
                    .Where(i =>
                        i.MedicineId == detail.MedicineId &&
                        i.Status == "ACTIVE" &&
                        i.StockQuantity > 0)
                    .OrderBy(i => i.ExpirationDate == null)
                    .ThenBy(i => i.ExpirationDate)
                    .ThenBy(i => i.ManufactureDate == null)
                    .ThenBy(i => i.ManufactureDate)
                    .ThenBy(i => i.InventoryId)
                    .ToListAsync();

                var available = batches.Sum(b => b.StockQuantity);
                if (available <= 0)
                {
                    throw new InvalidOperationException($"Out of stock for medicineId={detail.MedicineId}");
                }

                if (available < remaining)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for medicineId={detail.MedicineId}. Available={available}, Requested={remaining}");
                }

                foreach (var batch in batches)
                {
                    if (remaining <= 0)
                    {
                        break;
                    }

                    var take = Math.Min(batch.StockQuantity, remaining);
                    batch.StockQuantity -= take;
                    batch.LastUpdated = DateTime.UtcNow;

                    _db.TransactionDetails.Add(new TransactionDetail
                    {
                        TransactionId = transaction.TransactionId,
                        PrescriptionDetailId = detail.DetailId,
                        InventoryId = batch.InventoryId,
                        QuantityDispensed = take
                    });

                    remaining -= take;
                }
            }

            prescription.Status = isPartial ? PartiallyDispensedStatus : DispensedStatus;
            prescription.UpdatedAt = DateTime.UtcNow;
            transaction.Status = prescription.Status;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Success"] = "Dispensing completed successfully.";
            return RedirectToAction(nameof(Review), new { id = prescription.PrescriptionId });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["Error"] = $"Dispensing failed: {ex.Message}";
            return RedirectToAction(nameof(Review), new { id = vm.PrescriptionId });
        }
    }

    private async Task<int?> GetCurrentPharmacistId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            return null;
        }

        return await _db.Pharmacists
            .Where(p => p.UserId == userId)
            .Select(p => (int?)p.PharmacistId)
            .FirstOrDefaultAsync();
    }
}

