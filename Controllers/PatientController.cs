using DoctorAppointmentAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PatientController(AppDbContext db) { _db = db; }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get patient profile</summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var patient = await _db.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null) return NotFound(new { message = "Patient not found." });

            return Ok(new
            {
                patient.Id,
                patient.Name,
                patient.Phone,
                patient.Address,
                patient.CreatedAt,
                Email = patient.User.Email,
                LastLogin = patient.User.LastLogin
            });
        }

        /// <summary>Update patient profile</summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientDto dto)
        {
            var userId = GetUserId();
            var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return NotFound();

            patient.Name = dto.Name;
            patient.Phone = dto.Phone;
            patient.Address = dto.Address;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully." });
        }

        /// <summary>Get patient appointment count by status</summary>
        [HttpGet("{patientId}/summary")]
        public async Task<IActionResult> GetSummary(int patientId)
        {
            var appts = await _db.Appointments
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            return Ok(new
            {
                Total = appts.Count,
                Pending = appts.Count(a => a.Status == "Pending"),
                Confirmed = appts.Count(a => a.Status == "Confirmed"),
                Completed = appts.Count(a => a.Status == "Completed"),
                Cancelled = appts.Count(a => a.Status == "Cancelled")
            });
        }
    }

    public class UpdatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

}
