using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace truckPRO_api.Services
{
    public class EmailService : IEmailService
    {
        Task IEmailService.SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}
