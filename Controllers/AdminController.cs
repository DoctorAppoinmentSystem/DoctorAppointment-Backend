using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.DTOs.Admin;
using DoctorAppointmentAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IAdminService _adminService;

        public AdminController(AppDbContext db, IAdminService adminService)
        {
            _db = db;
            _adminService = adminService;
        }

        /// <summary>System statistics for admin dashboard</summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<StatisticsDto>> GetStatistics()
        {
            var dto = new StatisticsDto
            {
                TotalDoctors = await _db.Doctors.CountAsync(),
                TotalPatients = await _db.Patients.CountAsync(),
                TotalAppointments = await _db.Appointments.CountAsync(),
                PendingAppointments = await _db.Appointments.CountAsync(a => a.Status == "Pending"),
                ConfirmedAppointments = await _db.Appointments.CountAsync(a => a.Status == "Confirmed"),
                CompletedAppointments = await _db.Appointments.CountAsync(a => a.Status == "Completed"),
                CancelledAppointments = await _db.Appointments.CountAsync(a => a.Status == "Cancelled"),

                DoctorsBySpecialization = await _db.Doctors
                    .GroupBy(d => d.Specialization.Name)
                    .Select(g => new SpecializationCountDto
                    {
                        Specialization = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(),

                AppointmentsByMode = await _db.Appointments
                    .GroupBy(a => a.Mode)
                    .Select(g => new ModeCountDto
                    {
                        Mode = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync()
            };

            return Ok(dto);
        }

        /// <summary>Get all doctors</summary>
        [HttpGet("doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _db.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.User)
                .Select(d => new DTOs.Doctor.DoctorDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Email = d.User.Email,
                    Phone = d.Phone,
                    Address = d.Address,
                    SpecializationId = d.SpecializationId,
                    SpecializationName = d.Specialization.Name,
                    Degree = d.Degree,
                    Experience = d.Experience,
                    Mode = d.Mode,
                    IsAvailable = d.IsAvailable,
                    ProfilePhoto = d.ProfilePhoto,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>Get all patients</summary>
        [HttpGet("patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _db.Patients
                .Include(p => p.User)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Phone,
                    p.Address,
                    Email = p.User.Email,
                    p.User.LastLogin,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(patients);
        }

        /// <summary>Get activity logs (paged)</summary>
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var logs = await _adminService.GetLogsAsync(page, pageSize);
            return Ok(logs);
        }

        /// <summary>Toggle doctor availability</summary>
        [HttpPut("doctors/{id}/toggle")]
        public async Task<IActionResult> ToggleDoctor(int id)
        {
            var doctor = await _db.Doctors.FindAsync(id);
            if (doctor == null) return NotFound(new { message = "Doctor not found." });

            doctor.IsAvailable = !doctor.IsAvailable;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Doctor availability set to {doctor.IsAvailable}" });
        }

        /// <summary>Daily summary — appointments per mode/specialization</summary>
        [HttpGet("daily-summary")]
        public async Task<ActionResult<DailySummaryDto>> DailySummary([FromQuery] DateTime date)
        {
            var appts = await _db.Appointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .Include(a => a.Specialization)
                .ToListAsync();

            var summary = appts
                .GroupBy(a => new { a.Mode, a.Specialization.Name })
                .Select(g => new DailySummaryItemDto
                {
                    Mode = g.Key.Mode,
                    SpecializationName = g.Key.Name,
                    Count = g.Count(),
                    Confirmed = g.Count(a => a.Status == "Confirmed"),
                    Completed = g.Count(a => a.Status == "Completed"),
                    Pending = g.Count(a => a.Status == "Pending"),
                    Cancelled = g.Count(a => a.Status == "Cancelled")
                })
                .ToList();

            var dto = new DailySummaryDto
            {
                Date = date.Date,
                TotalAppointments = appts.Count,
                Summary = summary
            };

            return Ok(dto);
        }
    }
}