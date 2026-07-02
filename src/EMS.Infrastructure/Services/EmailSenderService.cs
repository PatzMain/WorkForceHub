using System;
using System.IO;
using System.Threading.Tasks;
using EMS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace EMS.Infrastructure.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(ILogger<EmailSenderService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Sending email to {Email} with subject {Subject}", email, subject);
            
            // For development purposes, write emails to a local log file inside the project directory
            try
            {
                var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sent_emails");
                Directory.CreateDirectory(directory);
                
                var filename = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{email}.html";
                var filepath = Path.Combine(directory, filename);
                
                File.WriteAllText(filepath, $"To: {email}\nSubject: {subject}\n\n{htmlMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write email log file");
            }

            return Task.CompletedTask;
        }
    }
}
