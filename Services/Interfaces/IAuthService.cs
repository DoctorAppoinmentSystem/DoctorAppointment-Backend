using DoctorAppointmentAPI.DTOs.Auth;

namespace DoctorAppointmentAPI.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>Register a new patient. Returns error message or null on success.</summary>
        Task<(bool Success, string Message)> RegisterPatientAsync(RegisterPatientDto dto);

        /// <summary>Register a new doctor. Returns error message or null on success.</summary>
        Task<(bool Success, string Message)> RegisterDoctorAsync(RegisterDoctorDto dto);

        /// <summary>
        /// Validate credentials and return JWT + profile info.
        /// Returns null if credentials are invalid or email is unverified.
        /// </summary>
        Task<LoginResultDto?> LoginAsync(LoginDto dto);

        /// <summary>Verify email address using the one-time token.</summary>
        Task<(bool Success, string Message)> VerifyEmailAsync(string token);
    }

    /// <summary>Payload returned after a successful login.</summary>
    public class LoginResultDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
