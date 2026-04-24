using DoctorAppointmentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique indexes
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Patient>().HasIndex(p => p.UserId).IsUnique();
            modelBuilder.Entity<Doctor>().HasIndex(d => d.UserId).IsUnique();

            // Cascade rules
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User).WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User).WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor).WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);

            // Seed specializations

        }
    }

}
