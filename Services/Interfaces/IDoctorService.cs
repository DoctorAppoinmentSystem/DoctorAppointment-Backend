using DoctorAppointmentAPI.Models;

namespace DoctorAppointmentAPI.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetDoctorsBySpecializationAsync(int specId, string mode);
        Task<Doctor?> GetDoctorProfileAsync(int doctorId);
        Task<bool> UpdateProfilePhotoAsync(int doctorId, string photoPath);
        Task<(bool, string)> UpdateDoctorProfileAsync(int doctorId, string name,
            string phone, string address, string degree, int experience);
        Task<List<AppointmentSummaryDto>> GetDoctorStatsAsync(int doctorId);
    }
    public class AppointmentSummaryDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }


}
