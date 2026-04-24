namespace DoctorAppointmentAPI.DTOs.Doctor
{
    public class DoctorAvailabilityDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        public string Mode { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Experience { get; set; }
        public List<string> BookedSlots { get; set; } = new();
        public int AvailableSlots { get; set; }


    }

}
