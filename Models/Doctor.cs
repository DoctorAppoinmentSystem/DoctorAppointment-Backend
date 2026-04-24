using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentAPI.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required]
        public int SpecializationId { get; set; }

        public string? Degree { get; set; }
        public int Experience { get; set; }
        public string? ProfilePhoto { get; set; }

        /// <summary>
        /// STRICT RULE: Online/Offline doctors are separate pools.
        /// </summary>
        [Required]
        public string Mode { get; set; } = string.Empty;  // Online | Offline

        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
