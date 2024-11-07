using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Smtp;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;

namespace truckPRO_api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string receiverEmail, string subject, string message)
        {
            // Set up SMTP sender with configuration from settings
            var sender = new SmtpSender(() => new SmtpClient()
            {
                Host = _configuration["SmtpSettings:Host"],
                Port = int.Parse(_configuration["SmtpSettings:Port"]),
                EnableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _configuration["SmtpSettings:Username"],
                    _configuration["SmtpSettings:Password"])
            });

            Email.DefaultSender = sender;

            var email = await Email
                .From(_configuration["SmtpSettings:Username"]) 
                .To(receiverEmail)
                .Subject(subject)
                .Body(message)
                .SendAsync();

            return email.Successful;
        }
    }
}