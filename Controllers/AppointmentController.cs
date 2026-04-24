using DoctorAppointmentAPI.DTOs.Appointment;
using DoctorAppointmentAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Book a new appointment (Patient only)</summary>
        [HttpPost("book")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Book([FromBody] BookAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Get patient ID from JWT
            var userId = GetUserId();
            // TODO: resolve patientId from userId via DB or include in token
            var (success, message, id) =
                await _appointmentService.BookAppointmentAsync(dto.PatientId, dto);

            return success ? Ok(new { message, appointmentId = id })
                           : BadRequest(new { message });
        }

        /// <summary>Get my appointments (Patient)</summary>
        [HttpGet("my/{patientId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments(int patientId)
        {
            var list = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(list);
        }

        /// <summary>Get doctor's appointments</summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DoctorAppointments(int doctorId)
        {
            var list = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
            return Ok(list);
        }

        /// <summary>Get all appointments (Admin)</summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllAppointments()
        {
            var list = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(list);
        }

        /// <summary>Update appointment status (Doctor/Admin)</summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var (success, message) =
                await _appointmentService.UpdateStatusAsync(id, dto.Status, GetUserId());
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>Get available doctors for a specialization/mode/date</summary>
        [HttpGet("available-doctors")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableDoctors(
    [FromQuery] int? specializationId,
    [FromQuery] string? mode,
    [FromQuery] DateTime? date)
        {
            var result = await _appointmentService
                .GetAvailableDoctorsAsync(specializationId, mode, date);

            return Ok(result);
        }
    }

}
