using System;
using System.Net.Mail;
using System.Text;

public class EmailService
{
    public bool SendEmail(string fromAddress, string toAddress, string subject, string body)
    {
        try
        {
            
            MailMessage mail = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };

            
            SmtpClient smtpClient = new("174.138.184.240")
            {
                Port = 25,                  
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true // No authentication
            };

            smtpClient.Send(mail);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
