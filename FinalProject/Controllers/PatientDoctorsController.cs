using FinalProject.Auth;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;

[Authorize(Roles = AppRoles.Patient)]
public class PatientDoctorsController : Controller
{
    private readonly HospitalManagementContext _db;

    public PatientDoctorsController(HospitalManagementContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? specialtyId = null, string? q = null)
    {
        ViewData["Specialties"] = await _db.Specialties.AsNoTracking()
            .OrderBy(s => s.SpecialtyName)
            .ToListAsync();
        ViewData["SelectedSpecialtyId"] = specialtyId;
        ViewData["Q"] = q ?? "";

        var query = _db.Doctors.AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .AsQueryable();

        if (specialtyId.HasValue)
        {
            query = query.Where(d => d.SpecialtyId == specialtyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(d =>
                d.User.FullName.Contains(term) ||
                d.User.Username.Contains(term) ||
                d.Specialty.SpecialtyName.Contains(term));
        }

        var doctors = await query
            .OrderBy(d => d.Specialty.SpecialtyName)
            .ThenBy(d => d.User.FullName)
            .ToListAsync();

        return View(doctors);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var doctor = await _db.Doctors.AsNoTracking()
            .Include(d => d.User)
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.DoctorId == id);

        if (doctor is null)
        {
            return NotFound();
        }

        return View(doctor);
    }
}

