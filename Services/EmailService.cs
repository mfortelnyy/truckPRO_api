using System;
using FluentEmail.Smtp;
using FluentEmail.Core;
using System.Net;
using System.Net.Mail;
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

        public async Task<bool> SendEmailAsync(string recieverEmail, string subject, string message)
        {
            
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis, 
                UseDefaultCredentials = true, 
                EnableSsl = false, 
                Timeout = 200, 
            });

            Email.DefaultSender = sender;

            // Send the email
            var email = await Email
                .From("mfortelnyy1@gmail.com") 
                .To(recieverEmail, "")         
                .Subject(subject)              
                .Body(message)                              
                .SendAsync();

            return email.Successful;
        }
    }
}
