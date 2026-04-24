using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorAppointmentAPI.Models
{

    public class Session
    {
        [Key] public int Id { get; set; }
        [Required] public int UserId { get; set; }
        [Required] public string Token { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime? LogoutTime { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

}
