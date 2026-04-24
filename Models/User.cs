using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;  // Plain text per project rule

        [Required]
        public string Role { get; set; } = string.Empty;  // Patient | Doctor | Admin

        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerifyToken { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }

}
