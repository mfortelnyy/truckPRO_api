using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace truckPRO_api.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            
            
            // Create a new SMTP client to send the email
            using (var client = new SmtpClient())
            {
                try
                {
                    // Connect to the SMTP server (replace with your actual settings)
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    // Authenticate with the SMTP server (replace with your actual email/password)
                    await client.AuthenticateAsync("fc.molochko@gmail.com", "Max11072001!");
                    
                 
                    // Send the email
                    await client.SendAsync(mm);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    //throw ex;  
                }
                finally
                {
                    // Disconnect the client and clean up
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
