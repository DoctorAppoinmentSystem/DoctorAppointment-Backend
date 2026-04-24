using DoctorAppointmentAPI.Models;

namespace DoctorAppointmentAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task LogActionAsync(int? userId, string action, string? entity = null,
            int? entityId = null, string? description = null, string? ip = null);
        Task<List<AppLog>> GetLogsAsync(int page, int pageSize);
    }

}
