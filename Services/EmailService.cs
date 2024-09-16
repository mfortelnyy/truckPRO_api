using System;
using FluentEmail.Smtp;
using System.Net.Mail;
using FluentEmail.Core;
using System.Net;


namespace truckPRO_api.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendEmailAsync(string recieverEmail, string subject, string message)
        {
            Console.WriteLine($"Here is {configuration["SmtpSettings:Host"]}, {configuration["SmtpSettings:Port"]}, {configuration["SmtpSettings:Username"]}");
            //setting should come from appsettings.json 
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                Host = configuration["SmtpSettings:Host"],

                Port = int.Parse(configuration["SmtpSettings:Port"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(configuration["SmtpSettings:Username"], configuration["SmtpSettings:Password"])
                
                /*
                //testing
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 567
                
                //DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                //PickupDirectoryLocation = @"C:\Games"
                */
            });

            Email.DefaultSender = sender;
            var email = await Email
                .From("mfortelnyy1@gmail.com")
                .To(recieverEmail, "Max")
                .Subject("Email Verification")
                .Body("Verification Code: ")
                .SendAsync();
            Console.WriteLine($"Email Successfull:  {email.ErrorMessages.ToString()}");

        }
    }
}
