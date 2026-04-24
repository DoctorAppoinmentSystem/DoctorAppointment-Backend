using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentAPI.Models
{
    public class AppLog
    {
        [Key] public int Id { get; set; }
        public int? UserId { get; set; }
        [Required] public string Action { get; set; } = string.Empty;
        public string? Entity { get; set; }
        public int? EntityId { get; set; }
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
