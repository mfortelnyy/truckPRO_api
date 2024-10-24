using System;
using FluentEmail.Smtp;
using System.Net.Mail;
using FluentEmail.Core;
using System.Net;
using Microsoft.Graph.Education.Classes.Item.Assignments.Item.Submissions.Item.Return;


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
                Timeout = 600000, //600 seconds to ensure all emails are sent 
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
