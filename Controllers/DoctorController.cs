using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IDoctorService _doctorService;

        public DoctorController(AppDbContext db, IDoctorService doctorService)
        {
            _db = db;
            _doctorService = doctorService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get current doctor profile</summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var doctor = await _db.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null) return NotFound(new { message = "Doctor profile not found." });

            return Ok(new
            {
                doctor.Id,
                doctor.Name,
                doctor.Phone,
                doctor.Address,
                doctor.Degree,
                doctor.Experience,
                doctor.ProfilePhoto,
                doctor.Mode,
                doctor.IsAvailable,
                Specialization = doctor.Specialization.Name,
                Email = doctor.User.Email
            });
        }

        /// <summary>Update doctor profile</summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateDoctorProfileDto dto)
        {
            var userId = GetUserId();
            var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return NotFound();

            var (success, message) = await _doctorService.UpdateDoctorProfileAsync(
                doctor.Id, dto.Name, dto.Phone, dto.Address, dto.Degree, dto.Experience);

            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>Upload profile photo</summary>
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowed.Contains(ext))
                return BadRequest(new { message = "Only image files are allowed." });

            var uploadPath = Path.Combine("wwwroot", "uploads", "photos");
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var userId = GetUserId();
            var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor != null)
            {
                doctor.ProfilePhoto = $"/uploads/photos/{fileName}";
                await _db.SaveChangesAsync();
            }

            return Ok(new { photoUrl = $"/uploads/photos/{fileName}" });
        }

        /// <summary>Get doctor's appointment statistics</summary>
        [HttpGet("stats/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetStats(int doctorId)
        {
            var stats = await _doctorService.GetDoctorStatsAsync(doctorId);
            return Ok(stats.FirstOrDefault());
        }
    }

    public class UpdateDoctorProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Experience { get; set; }
    }

}
