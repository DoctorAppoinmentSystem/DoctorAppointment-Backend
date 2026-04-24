namespace DoctorAppointmentAPI.DTOs.Admin
{
    /// <summary>
    /// Returned by GET /api/admin/daily-summary?date=...
    /// </summary>
    public class DailySummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalAppointments { get; set; }
        public List<DailySummaryItemDto> Summary { get; set; } = new();
    }

    public class DailySummaryItemDto
    {
        /// <summary>Online | Offline</summary>
        public string Mode { get; set; } = string.Empty;

        /// <summary>Specialization name</summary>
        public string SpecializationName { get; set; } = string.Empty;

        public int Count { get; set; }
        public int Confirmed { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
    }
}