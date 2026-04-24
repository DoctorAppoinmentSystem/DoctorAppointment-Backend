namespace DoctorAppointmentAPI.DTOs.Doctor
{
    /// <summary>
    /// Full doctor profile returned to clients.
    /// Replaces anonymous projections scattered across controllers.
    /// </summary>
    public class DoctorDto
    {
        public int Id { get; set; }

        // ── Identity ─────────────────────────────────────────────────────────
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // ── Professional ─────────────────────────────────────────────────────
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set; } = string.Empty;
        public string? Degree { get; set; }
        public int Experience { get; set; }

        // ── Operational ──────────────────────────────────────────────────────
        /// <summary>Online | Offline</summary>
        public string Mode { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }

        public string? ProfilePhoto { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Appointment stats summary for a doctor's dashboard.
    /// </summary>
    public class AppointmentSummaryDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }
}