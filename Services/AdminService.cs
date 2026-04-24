using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.Models;
using DoctorAppointmentAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace DoctorAppointmentAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _db;
        public AdminService(AppDbContext db) { _db = db; }

        /// <summary>Record an activity log entry.</summary>
        public async Task LogActionAsync(int? userId, string action, string? entity = null,
            int? entityId = null, string? description = null, string? ip = null)
        {
            var log = new AppLog
            {
                UserId = userId,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Description = description,
                IpAddress = ip,
                CreatedAt = DateTime.UtcNow
            };
            _db.AppLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AppLog>> GetLogsAsync(int page, int pageSize)
        {
            return await _db.AppLogs
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }

}
