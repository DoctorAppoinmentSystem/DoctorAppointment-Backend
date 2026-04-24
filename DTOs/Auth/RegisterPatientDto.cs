using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentAPI.DTOs.Auth
{
    public class RegisterPatientDto
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [MaxLength(20)] public string? Phone { get; set; }
        public string? Address { get; set; }
    }

}
