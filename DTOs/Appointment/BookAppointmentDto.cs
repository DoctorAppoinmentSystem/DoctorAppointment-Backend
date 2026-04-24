using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentAPI.DTOs.Appointment
{
    public class BookAppointmentDto
    {
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        [Required] public int SpecializationId { get; set; }
        [Required] public DateTime AppointmentDate { get; set; }
        [Required] public string AppointmentTime { get; set; } = string.Empty;
        [Required] public string Mode { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
    public class UpdateStatusDto
    {
        [Required] public string Status { get; set; } = string.Empty;
    }

}
