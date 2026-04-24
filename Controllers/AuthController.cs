using DoctorAppointmentAPI.DTOs.Auth;
using DoctorAppointmentAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Register a new Patient</summary>
        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _authService.RegisterPatientAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>Register a new Doctor</summary>
        [HttpPost("register/doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _authService.RegisterDoctorAsync(dto);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }

        /// <summary>Login for all roles</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid credentials or email not verified." });

            return Ok(new
            {
                result.Token,
                result.Role,
                result.UserId,
                result.ProfileId,
                result.Name
            });
        }

        /// <summary>Verify email with one-time token</summary>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var (success, message) = await _authService.VerifyEmailAsync(token);
            return success ? Ok(new { message }) : BadRequest(new { message });
        }
    }
}
