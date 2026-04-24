using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.Models;
using DoctorAppointmentAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace DoctorAppointmentAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _db;
        public DoctorService(AppDbContext db) { _db = db; }

        public async Task<List<Doctor>> GetDoctorsBySpecializationAsync(int specId, string mode)
        {
            return await _db.Doctors
                .Where(d => d.SpecializationId == specId && d.Mode == mode && d.IsAvailable)
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .ToListAsync();
        }

        public async Task<Doctor?> GetDoctorProfileAsync(int doctorId)
        {
            return await _db.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<bool> UpdateProfilePhotoAsync(int doctorId, string photoPath)
        {
            var doctor = await _db.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;
            doctor.ProfilePhoto = photoPath;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool, string)> UpdateDoctorProfileAsync(int doctorId,
            string name, string phone, string address, string degree, int experience)
        {
            var doctor = await _db.Doctors.FindAsync(doctorId);
            if (doctor == null) return (false, "Doctor not found.");
            doctor.Name = name;
            doctor.Phone = phone;
            doctor.Address = address;
            doctor.Degree = degree;
            doctor.Experience = experience;
            await _db.SaveChangesAsync();
            return (true, "Profile updated successfully.");
        }

        public async Task<List<AppointmentSummaryDto>> GetDoctorStatsAsync(int doctorId)
        {
            var appointments = await _db.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            var summary = new AppointmentSummaryDto
            {
                Total = appointments.Count,
                Pending = appointments.Count(a => a.Status == "Pending"),
                Confirmed = appointments.Count(a => a.Status == "Confirmed"),
                Completed = appointments.Count(a => a.Status == "Completed"),
                Cancelled = appointments.Count(a => a.Status == "Cancelled")
            };

            return new List<AppointmentSummaryDto> { summary };
        }
    }

}
