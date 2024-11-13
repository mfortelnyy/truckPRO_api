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
        private readonly string logoUrl = "https://truckphotos.s3.us-east-2.amazonaws.com/email_logo.png";
        private readonly string fromAddress = "TruckPro <no_reply>";



        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendWelcomeEmailAsync(string receiverEmail)
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
                .From(fromAddress) 
                .To(receiverEmail)
                .Subject("TruckPro Registration Invitation")
                .Body($@"
                    <html>
                    <body>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                            <table width='100%'>
                                <tr>
                                    <td style='text-align: center;'>
                                        <img src='{logoUrl}' alt='TruckPro Logo' width='150'/>
                                    </td>
                                </tr>
                            </table>
                            <p>Dear Driver,</p>
                            <p>Welcome to TruckPro! Please complete your registration using the link below:</p>
                            <a href=''>Register Here</a>
                            <p>Best regards,<br/>The TruckPro Team</p>
                        </div>
                    </body>
                    </html>", isHtml: true)
                .SendAsync();

            return email.Successful;
        }

        public async Task<bool> ReSendVerification(string receiverEmail, string verificationCode)
        {
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
                .From(fromAddress) 
                .To(receiverEmail)
                .Subject("TruckPro Registration Verification Code")
                .Body($@"
                    <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                            <table width='100%'>
                                <tr>
                                    <td style='text-align: center;'>
                                        <img src='{logoUrl}' alt='TruckPro Logo' width='150'/>
                                    </td>
                                </tr>
                            </table>
                            <h2 style='color: #555;'>Dear Driver,</h2>
                            <p>Welcome to TruckPro! To complete your registration, please enter the verification code below:</p>
                            <h1 style='background-color: #f2f2f2; padding: 10px; text-align: center; border-radius: 4px; color: #333;'>{verificationCode}</h1>
                            <p>If you did not request this code, please ignore this email.</p>
                            <p style='color: #888;'>Best regards,<br/>The TruckPro Team</p>
                            <hr style='border: none; border-top: 1px solid #e0e0e0; margin-top: 20px;'/>
                            <p style='font-size: 12px; color: #999; text-align: center;'>
                                TruckPro Inc.<br/>
                                 <br/>
                                <a href='mailto:support@truckpro.com' style='color: #999;'>support@truckpro.com</a>
                            </p>
                        </div>
                    </body>
                    </html>", isHtml: true)
                .SendAsync();

        return email.Successful;
        }

        public async Task<bool> SendTemporaryPassword(string receiverEmail, string temporaryPassword)
        {
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
                .From(fromAddress) 
                .To(receiverEmail)
                .Subject("TruckPro Password Reset")
                .Body($@"
                    <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                            <table width='100%'>
                                <tr>
                                    <td style='text-align: center;'>
                                        <img src='{logoUrl}' alt='TruckPro Logo' width='150'/>
                                    </td>
                                </tr>
                            </table>
                            <h2 style='color: #555;'>Password Reset Request</h2>
                            <p>Hello,</p>
                            <p>We received a request to reset your password. Please use the temporary password below to log in. Once logged in, we strongly recommend updating your password.</p>
                            <h1 style='background-color: #f2f2f2; padding: 10px; text-align: center; border-radius: 4px; color: #333;'>{temporaryPassword}</h1>
                            <p>If you did not request a password reset, please ignore this email or contact support.</p>
                            <p style='color: #888;'>Best regards,<br/>The TruckPro Team</p>
                            <hr style='border: none; border-top: 1px solid #e0e0e0; margin-top: 20px;'/>
                            <p style='font-size: 12px; color: #999; text-align: center;'>
                                TruckPro Inc.<br/>
                                 <br/>
                                <a href='mailto:support@truckpro.com' style='color: #999;'>support@truckpro.com</a>
                            </p>
                        </div>
                    </body>
                    </html>", isHtml: true)
                .SendAsync();

                return email.Successful;
            }
        }
            
    }