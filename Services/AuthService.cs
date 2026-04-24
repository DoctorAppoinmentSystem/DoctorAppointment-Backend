using DoctorAppointmentAPI.Data;
using DoctorAppointmentAPI.DTOs.Auth;
using DoctorAppointmentAPI.Helpers;
using DoctorAppointmentAPI.Models;
using DoctorAppointmentAPI.Services.Interfaces;

namespace DoctorAppointmentAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(AppDbContext db, IConfiguration config, IEmailService emailService)
        {
            _db = db;
            _config = config;
            _emailService = emailService;
        }

        // ── Register Patient ─────────────────────────────────────────────────

        public async Task<(bool Success, string Message)> RegisterPatientAsync(RegisterPatientDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, "Email already registered.");

            var verifyToken = Guid.NewGuid().ToString();

            var user = new User
            {
                Email = dto.Email,
                Password = dto.Password,   // Plain text per project rule
                Role = "Patient",
                EmailVerifyToken = verifyToken
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _db.Patients.Add(new Patient
            {
                UserId = user.Id,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address
            });
            await _db.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(dto.Email, verifyToken);

            return (true, "Registered. Please verify your email.");
        }

        // ── Register Doctor ──────────────────────────────────────────────────

        public async Task<(bool Success, string Message)> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return (false, "Email already registered.");

            var verifyToken = Guid.NewGuid().ToString();

            var user = new User
            {
                Email = dto.Email,
                Password = dto.Password,
                Role = "Doctor",
                EmailVerifyToken = verifyToken
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _db.Doctors.Add(new Doctor
            {
                UserId = user.Id,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                SpecializationId = dto.SpecializationId,
                Degree = dto.Degree,
                Experience = dto.Experience,
                Mode = dto.Mode   // Online | Offline — STRICT
            });
            await _db.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(dto.Email, verifyToken);

            return (true, "Doctor registered. Please verify your email.");
        }

        // ── Login ────────────────────────────────────────────────────────────

        public async Task<LoginResultDto?> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

            if (user == null || !user.IsEmailVerified)
                return null;

            user.LastLogin = DateTime.UtcNow;

            var token = JwtHelper.GenerateToken(user.Id, user.Email, user.Role, _config);

            _db.Sessions.Add(new Session
            {
                UserId = user.Id,
                Token = token,
                LoginTime = DateTime.UtcNow,
                IsActive = true
            });
            await _db.SaveChangesAsync();

            int profileId = 0;
            string name = "Admin";

            if (user.Role == "Patient")
            {
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                profileId = patient?.Id ?? 0;
                name = patient?.Name ?? string.Empty;
            }
            else if (user.Role == "Doctor")
            {
                var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                profileId = doctor?.Id ?? 0;
                name = doctor?.Name ?? string.Empty;
            }

            return new LoginResultDto
            {
                Token = token,
                Role = user.Role,
                UserId = user.Id,
                ProfileId = profileId,
                Name = name
            };
        }

        // ── Verify Email ─────────────────────────────────────────────────────

        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailVerifyToken == token);
            if (user == null)
                return (false, "Invalid or expired token.");

            user.IsEmailVerified = true;
            user.EmailVerifyToken = null;
            await _db.SaveChangesAsync();

            return (true, "Email verified. You can now login.");
        }
    }
}
