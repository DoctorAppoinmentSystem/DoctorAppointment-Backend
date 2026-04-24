namespace DoctorAppointmentAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendVerificationEmailAsync(string to, string token);
        Task SendAppointmentConfirmationAsync(string to, string patientName,
            string doctorName, DateTime date, string time, string mode);
        Task SendAppointmentCancellationAsync(string to, string patientName,
            string doctorName, DateTime date);
        Task SendDoctorResponseAsync(string to, string patientName,
            string doctorName, string status);
        Task SendReminderAsync(string to, string patientName,
            string doctorName, DateTime date, string time, string dayLabel);
    }
}
