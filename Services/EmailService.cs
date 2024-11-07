using System;
using FluentEmail.Smtp;
using FluentEmail.Core;
using System.Net.Mail;


namespace truckPRO_api.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {

        public async Task<bool> SendEmailAsync(string recieverEmail, string subject, string message)
        {
            //Console.WriteLine($"Here is {configuration["SmtpSettings:Host"]}, {configuration["SmtpSettings:Port"]}, {configuration["SmtpSettings:Username"]}");
             
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                Host = configuration["SmtpSettings:Host"],

                Port = int.Parse(configuration["SmtpSettings:Port"]),
                Timeout = 600, //600 seconds to ensure all emails are sent 
                EnableSsl = false,
                UseDefaultCredentials = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //Credentials = new NetworkCredential(configuration["SmtpSettings:Username"], configuration["SmtpSettings:Password"])
                
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
                .From("do_not_reply@truckcheck.org")
                .To(recieverEmail, "")
                .Subject(subject)
                .Body(message)
                .SendAsync();
            if(email.Successful)
            {
                return true;
            }  
            else
            {
                return false;
            }
            //Console.WriteLine($"Email Successfull:  {email.ErrorMessages.ToString()}");

        }
        

    }
}
