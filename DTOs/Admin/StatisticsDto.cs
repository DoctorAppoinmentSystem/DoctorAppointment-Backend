namespace DoctorAppointmentAPI.DTOs.Admin
{
    /// <summary>
    /// Returned by GET /api/admin/statistics
    /// </summary>
    public class StatisticsDto
    {
        // ── Counts ──────────────────────────────────────────────────────────
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        // ── Breakdowns ───────────────────────────────────────────────────────
        /// <summary>Number of doctors grouped by specialization</summary>
        public List<SpecializationCountDto> DoctorsBySpecialization { get; set; } = new();

        /// <summary>Number of appointments grouped by mode (Online / Offline)</summary>
        public List<ModeCountDto> AppointmentsByMode { get; set; } = new();
    }

    public class SpecializationCountDto
    {
        public string Specialization { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ModeCountDto
    {
        /// <summary>Online | Offline</summary>
        public string Mode { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}