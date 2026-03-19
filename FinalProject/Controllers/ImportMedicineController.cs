using System.Security.Claims;
using FinalProject.Auth;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Pharmacist)]
public class ImportMedicineController : Controller
{
    private readonly HospitalManagementContext _db;

    public ImportMedicineController(HospitalManagementContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ImportLookupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(ImportLookupViewModel vm)
    {
        if (!ModelState.IsValid || vm.OrderId is null)
        {
            return View(vm);
        }

        return RedirectToAction(nameof(Receive), new { id = vm.OrderId.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Receive(int id)
    {
        var po = await _db.PurchaseOrders
            .AsNoTracking()
            .Include(o => o.Supplier)
            .Include(o => o.PurchaseOrderDetails)
            .ThenInclude(d => d.Medicine)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (po is null)
        {
            return NotFound("Order not found");
        }

        var vm = new ImportReceiveViewModel
        {
            OrderId = po.OrderId,
            OrderStatus = po.Status,
            SupplierId = po.SupplierId,
            SupplierName = po.Supplier.SupplierName,
            Notes = null,
            Items = po.PurchaseOrderDetails.Select(d => new ImportReceiveItemViewModel
            {
                PurchaseOrderDetailId = d.DetailId,
                MedicineId = d.MedicineId,
                MedicineName = d.Medicine.MedicineName,
                OrderedQty = d.QuantityOrdered,
                AlreadyReceivedQty = d.QuantityReceived ?? 0,
                ReceiveQty = Math.Max(0, d.QuantityOrdered - (d.QuantityReceived ?? 0)),
                BatchNumber = "",
                ManufactureDate = null,
                ExpirationDate = null,
                UnitCost = d.UnitPrice,
                SellingPrice = null
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(ImportReceiveViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var pharmacistId = await GetCurrentPharmacistId();
        if (pharmacistId is null)
        {
            return Forbid();
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var po = await _db.PurchaseOrders
                .Include(o => o.PurchaseOrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == vm.OrderId);

            if (po is null)
            {
                return NotFound("Order not found");
            }

            // Create GRN header
            var grn = new GoodsReceiptNote
            {
                OrderId = po.OrderId,
                PharmacistId = pharmacistId.Value,
                Status = "COMPLETED",
                Notes = vm.Notes
            };
            _db.GoodsReceiptNotes.Add(grn);
            await _db.SaveChangesAsync(); // get GrnId

            var podById = po.PurchaseOrderDetails.ToDictionary(d => d.DetailId);

            foreach (var item in vm.Items)
            {
                if (!podById.TryGetValue(item.PurchaseOrderDetailId, out var pod))
                {
                    throw new InvalidOperationException("Invalid purchase order detail.");
                }

                var receiveQty = item.ReceiveQty;
                if (receiveQty < 0)
                {
                    throw new InvalidOperationException("Invalid receive quantity.");
                }

                if (receiveQty == 0)
                {
                    continue;
                }

                if (item.ExpirationDate is null)
                {
                    throw new InvalidOperationException("Expiration date is required.");
                }

                if (item.ExpirationDate.Value < DateOnly.FromDateTime(DateTime.Today))
                {
                    throw new InvalidOperationException("Cannot add expired stock.");
                }

                // Create inventory batch (physical stock)
                var inv = new Inventory
                {
                    MedicineId = item.MedicineId,
                    BatchNumber = item.BatchNumber?.Trim(),
                    ManufactureDate = item.ManufactureDate,
                    ExpirationDate = item.ExpirationDate,
                    StockQuantity = receiveQty,
                    UnitCost = item.UnitCost,
                    SellingPrice = item.SellingPrice,
                    Location = null,
                    Status = "ACTIVE",
                    LastUpdated = DateTime.UtcNow
                };
                _db.Inventories.Add(inv);
                await _db.SaveChangesAsync(); // get InventoryId

                // Link GRN detail to inventory and PO detail
                _db.GoodsReceiptDetails.Add(new GoodsReceiptDetail
                {
                    GrnId = grn.GrnId,
                    PurchaseOrderDetailId = pod.DetailId,
                    MedicineId = item.MedicineId,
                    BatchNumber = inv.BatchNumber,
                    ManufactureDate = inv.ManufactureDate,
                    ExpirationDate = inv.ExpirationDate,
                    QuantityReceived = receiveQty,
                    UnitCost = item.UnitCost,
                    InventoryId = inv.InventoryId
                });

                pod.QuantityReceived = (pod.QuantityReceived ?? 0) + receiveQty;
            }

            // Update PO status
            var isCompleted = po.PurchaseOrderDetails.All(d => (d.QuantityReceived ?? 0) >= d.QuantityOrdered);
            po.Status = isCompleted ? "COMPLETED" : "PARTIALLY_RECEIVED";

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Success"] = "Import completed successfully.";
            return RedirectToAction(nameof(Receive), new { id = po.OrderId });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["Error"] = $"Import failed: {ex.Message}";
            return RedirectToAction(nameof(Receive), new { id = vm.OrderId });
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

