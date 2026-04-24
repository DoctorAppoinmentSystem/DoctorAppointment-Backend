using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecializationController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SpecializationController(AppDbContext db) { _db = db; }

        /// <summary>Get all specializations grouped by category</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var specs = await _db.Specializations
                .Select(s => new {
                    s.Id,
                    s.Name,
                    s.Category,
                    DoctorCount = s.Doctors.Count(d => d.IsAvailable)
                })
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Name)
                .ToListAsync();
            return Ok(specs);
        }

        /// <summary>Add new specialization (Admin only)</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] Specialization spec)
        {
            _db.Specializations.Add(spec);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Specialization added.", id = spec.Id });
        }
    }

}
