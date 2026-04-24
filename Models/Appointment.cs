using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        [Required] public int SpecializationId { get; set; }

        [Required] public DateTime AppointmentDate { get; set; }
        [Required] public string AppointmentTime { get; set; } = string.Empty;

        [Required] public string Mode { get; set; } = string.Empty;  // Online | Offline

        /// <summary>Pending | Confirmed | Cancelled | Completed | NoShow</summary>
        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        // Reminder tracking flags
        public bool ReminderSent2Day { get; set; } = false;
        public bool ReminderSent1Day { get; set; } = false;
        public bool ReminderSentSameDay { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PatientId")] public Patient Patient { get; set; } = null!;
        [ForeignKey("DoctorId")] public Doctor Doctor { get; set; } = null!;
        [ForeignKey("SpecializationId")] public Specialization Specialization { get; set; } = null!;
    }

}
