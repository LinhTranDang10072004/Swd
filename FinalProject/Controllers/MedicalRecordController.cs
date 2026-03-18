using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly HospitalManagementContext _context;

        public MedicalRecordController(HospitalManagementContext context)
        {
            _context = context;
        }

        // M1, M3, M4: Tìm kiếm và chọn bệnh nhân,Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            var patients = await _context.Patients
                .Where(p => string.IsNullOrEmpty(searchTerm) || p.FullName.Contains(searchTerm) || p.Phone.Contains(searchTerm))
                .ToListAsync();
            return View(patients);
        }

        // M7, M9, M10: Lấy danh sách hồ sơ bệnh án của bệnh nhân (getMedicalRecordList)
        public async Task<IActionResult> History(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null) return NotFound();

            var records = await _context.MedicalRecords
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.VisitDate)
                .ToListAsync();

            ViewBag.Patient = patient;

            // Alternative Sequence A1: No Existing Medical Record
            if (!records.Any())
            {
                ViewBag.Message = "The patient has no medical records.";
            }

            return View(records);
        }

        // M11, M12: Form tạo mới hoặc chỉnh sửa
        public IActionResult Create(int patientId)
        {
            var model = new MedicalRecord
            {
                PatientId = patientId,
                VisitDate = DateTime.Now,
                Status = "OPEN"
            };
            return View(model);
        }

        // M13, M15, M17: Xử lý lưu dữ liệu (createMedicalRecord / saveMedicalRecord)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.Diagnosis))
            {
                ModelState.AddModelError("Diagnosis", "Diagnosis is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    record.CreatedAt = DateTime.Now;
                    record.DoctorId = 1;

                    _context.Add(record);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Medical record created successfully";
                    return RedirectToAction(nameof(History), new { patientId = record.PatientId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException?.Message ?? ex.Message);
                }
            }

            return View(record);
        }

        // Xem chi tiết (View-only mode - Alternative A2)
        public async Task<IActionResult> Details(int id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Doctor)
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(m => m.RecordId == id);

            if (record == null) return NotFound();

            return View(record);
        }

        // GET: MedicalRecord/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(m => m.RecordId == id);

            if (record == null) return NotFound();

            // Kiểm tra Alternative Sequence A2: Nếu hồ sơ đã bị khóa (Locked) thì không cho sửa
            if (record.Status == "LOCKED" || record.Status == "CLOSED" || record.Status == "ARCHIVED")
            {
                TempData["Error"] = "This medical record cannot be edited.";
                return RedirectToAction(nameof(Details), new { id = record.RecordId });
            }

            return View(record);
        }

        // POST: MedicalRecord/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecord record)
        {
            if (id != record.RecordId) return NotFound();

            // Re-validate logic (Operation 3: validateMedicalRecord)
            if (string.IsNullOrEmpty(record.Diagnosis))
            {
                ModelState.AddModelError("Diagnosis", "Diagnosis is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    record.UpdatedAt = DateTime.Now; // Cập nhật thời gian chỉnh sửa
                    _context.Update(record);
                    await _context.SaveChangesAsync(); // Operation 4: saveMedicalRecord (Update mode)

                    TempData["Success"] = "Medical record updated successfully!";
                    return RedirectToAction(nameof(History), new { patientId = record.PatientId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "The record was updated by another user. Please reload.");
                }
            }

            // Alternative Sequence A3: Dữ liệu không hợp lệ
            ViewBag.Error = "Incorrect format, doctor needs to redo it.";
            return View(record);
        }
    }
}
