using DoctorAppointmentAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DoctorAppointmentAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _config["Email:FromName"], _config["Email:FromAddress"]));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_config["Email:Host"],
                    int.Parse(_config["Email:Port"]!), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(
                    _config["Email:Username"], _config["Email:Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
            }
        }

        public async Task SendVerificationEmailAsync(string to, string token)
        {
            string link = $"http://localhost:4200/verify-email?token={token}";
            string body = $@"
                <h2>Email Verification</h2>
                <p>Click the link below to verify your account:</p>
                <a href='{link}' style='background:#1E3A5F;color:#fff;padding:10px 20px;
                  text-decoration:none;border-radius:4px;'>Verify Email</a>
                <p>Link: {link}</p>";
            await SendEmailAsync(to, "Verify Your Email - Doctor Appointment System", body);
        }

        public async Task SendAppointmentConfirmationAsync(string to, string patientName,
            string doctorName, DateTime date, string time, string mode)
        {
            string body = $@"
                <h2 style='color:#1E3A5F;'>Appointment Confirmed!</h2>
                <p>Dear {patientName},</p>
                <p>Your appointment has been booked successfully.</p>
                <table style='border-collapse:collapse;'>
                    <tr><td><b>Doctor:</b></td><td>{doctorName}</td></tr>
                    <tr><td><b>Date:</b></td><td>{date:dd MMM yyyy}</td></tr>
                    <tr><td><b>Time:</b></td><td>{time}</td></tr>
                    <tr><td><b>Mode:</b></td><td>{mode}</td></tr>
                </table>
                <p>Thank you for choosing our clinic!</p>";
            await SendEmailAsync(to, "Appointment Booking Confirmation", body);
        }

        public async Task SendAppointmentCancellationAsync(string to, string patientName,
            string doctorName, DateTime date)
        {
            string body = $@"
                <h2 style='color:#cc0000;'>Appointment Cancelled</h2>
                <p>Dear {patientName},</p>
                <p>Your appointment with <b>{doctorName}</b> on <b>{date:dd MMM yyyy}</b>
                   has been cancelled.</p>
                <p>Please book a new appointment if needed.</p>";
            await SendEmailAsync(to, "Appointment Cancellation Confirmation", body);
        }

        public async Task SendDoctorResponseAsync(string to, string patientName,
            string doctorName, string status)
        {
            string color = status == "Confirmed" ? "#007700" : "#cc0000";
            string body = $@"
                <h2 style='color:{color};'>Appointment {status}</h2>
                <p>Dear {patientName},</p>
                <p>Dr. {doctorName} has <b>{status.ToLower()}</b> your appointment.</p>";
            await SendEmailAsync(to, $"Appointment {status} by Doctor", body);
        }

        public async Task SendReminderAsync(string to, string patientName,
            string doctorName, DateTime date, string time, string dayLabel)
        {
            string body = $@"
                <h2 style='color:#1E3A5F;'>Appointment Reminder</h2>
                <p>Dear {patientName},</p>
                <p>This is a reminder that you have an appointment <b>{dayLabel}</b>.</p>
                <table>
                    <tr><td><b>Doctor:</b></td><td>{doctorName}</td></tr>
                    <tr><td><b>Date:</b></td><td>{date:dd MMM yyyy}</td></tr>
                    <tr><td><b>Time:</b></td><td>{time}</td></tr>
                </table>";
            await SendEmailAsync(to, $"Appointment Reminder - {dayLabel}", body);
        }
    }
}
