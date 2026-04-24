using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.Models;
using DoctorAppointmentAPI.DTOs.Appointment;
using Microsoft.EntityFrameworkCore;
using DoctorAppointmentAPI.Services.Interfaces;
using DoctorAppointmentAPI.DTOs.Doctor;
namespace DoctorAppointmentAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;

        public AppointmentService(AppDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<(bool, string, int)> BookAppointmentAsync(
            int patientId, BookAppointmentDto dto)
        {
            // Validate doctor exists with matching mode
            var doctor = await _db.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId && d.Mode == dto.Mode);
            if (doctor == null)
                return (false, "Doctor not found or mode mismatch.", 0);

            // Check for conflicting slot
            bool slotTaken = await _db.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentDate.Date == dto.AppointmentDate.Date &&
                a.AppointmentTime == dto.AppointmentTime &&
                (a.Status == "Pending" || a.Status == "Confirmed"));
            if (slotTaken)
                return (false, "This time slot is already booked.", 0);

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                SpecializationId = dto.SpecializationId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Mode = dto.Mode,
                Status = "Pending",
                Notes = dto.Notes
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            // Send confirmation email
            var patient = await _db.Patients.Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient != null)
            {
                await _emailService.SendAppointmentConfirmationAsync(
                    patient.User.Email, patient.Name,
                    doctor.Name, dto.AppointmentDate, dto.AppointmentTime, dto.Mode);
            }

            return (true, "Appointment booked successfully.", appointment.Id);
        }

        public async Task<(bool, string)> UpdateStatusAsync(
            int appointmentId, string status, int actorUserId)
        {
            var appt = await _db.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appt == null) return (false, "Appointment not found.");

            appt.Status = status;
            appt.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Notify patient
            if (status is "Confirmed" or "Cancelled")
            {
                if (status == "Cancelled")
                    await _emailService.SendAppointmentCancellationAsync(
                        appt.Patient.User.Email, appt.Patient.Name,
                        appt.Doctor.Name, appt.AppointmentDate);
                else
                    await _emailService.SendDoctorResponseAsync(
                        appt.Patient.User.Email, appt.Patient.Name,
                        appt.Doctor.Name, status);
            }
            return (true, $"Status updated to {status}.");
        }

        public async Task<List<AppointmentResponseDto>> GetPatientAppointmentsAsync(int patientId)
        {
            return await _db.Appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .Include(a => a.Specialization)
                .Select(a => MapToDto(a))
                .ToListAsync();
        }

        public async Task<List<AppointmentResponseDto>> GetDoctorAppointmentsAsync(int doctorId)
        {
            return await _db.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient)
                .Include(a => a.Specialization)
                .Select(a => MapToDto(a))
                .ToListAsync();
        }

        public async Task<List<AppointmentResponseDto>> GetAllAppointmentsAsync()
        {
            return await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Specialization)
                .Select(a => MapToDto(a))
                .ToListAsync();
        }

        public async Task<List<DoctorAvailabilityDto>> GetAvailableDoctorsAsync(
    int? specializationId, string? mode, DateTime? date)
        {
            var query = _db.Doctors
                    .Include(d => d.Specialization)   // ✅ ADD THIS
                .Include(d => d.Appointments)
                .Where(d => d.IsAvailable)
                .AsQueryable();

            // ✅ Filter by specialization
            if (specializationId.HasValue && specializationId.Value > 0)
                query = query.Where(d => d.SpecializationId == specializationId.Value);

            // ✅ FIX: MODE LOGIC (HANDLE BOTH)
            if (!string.IsNullOrEmpty(mode))
            {
                query = query.Where(d =>
                    d.Mode == mode || d.Mode == "Both"
                );
            }

            var doctors = await query.ToListAsync();

            return doctors.Select(d => new DoctorAvailabilityDto
            {
                DoctorId = d.Id,
                DoctorName = d.Name,

                // ✅ ADD THIS LINE
                Specialization = d.Specialization.Name,

                Mode = d.Mode,
                Degree = d.Degree ?? "",
                Experience = d.Experience,

                BookedSlots = date.HasValue
        ? d.Appointments
            .Where(a => a.AppointmentDate.Date == date.Value.Date &&
                        (a.Status == "Pending" || a.Status == "Confirmed"))
            .Select(a => a.AppointmentTime)
            .ToList()
        : new List<string>(),

                AvailableSlots = date.HasValue
        ? 13 - d.Appointments.Count(a =>
            a.AppointmentDate.Date == date.Value.Date &&
            (a.Status == "Pending" || a.Status == "Confirmed"))
        : 13
            }).ToList();
        }

        private static AppointmentResponseDto MapToDto(Appointment a) => new()
        {
            Id = a.Id,
            PatientName = a.Patient?.Name ?? "",
            DoctorName = a.Doctor?.Name ?? "",
            Specialization = a.Specialization?.Name ?? "",
            AppointmentDate = a.AppointmentDate,
            AppointmentTime = a.AppointmentTime,
            Mode = a.Mode,
            Status = a.Status,
            Notes = a.Notes ?? ""
        };
    }
}


