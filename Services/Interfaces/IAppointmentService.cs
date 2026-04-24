using DoctorAppointmentAPI.DTOs.Appointment;
using DoctorAppointmentAPI.DTOs.Doctor;

namespace DoctorAppointmentAPI.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<(bool Success, string Message, int AppointmentId)> BookAppointmentAsync(
             int patientId, BookAppointmentDto dto);
        Task<List<AppointmentResponseDto>> GetPatientAppointmentsAsync(int patientId);
        Task<List<AppointmentResponseDto>> GetDoctorAppointmentsAsync(int doctorId);
        Task<List<AppointmentResponseDto>> GetAllAppointmentsAsync();
        Task<(bool, string)> UpdateStatusAsync(int appointmentId, string status, int actorUserId);
        Task<List<DoctorAvailabilityDto>> GetAvailableDoctorsAsync(
    int? specializationId, string? mode, DateTime? date);

    }

}
