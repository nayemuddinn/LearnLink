using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LearnLink.Services
{
    public class EmailService
    {
        public static async Task<bool> SendVerificationEmailAsync(string toEmail, string verificationLink)
        {
            try
            {
                var smtpHost = "smtp-relay.brevo.com";
                var smtpPort = 587;
                var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
                var smtpPass = ConfigurationManager.AppSettings["SmtpPass"];
                var fromEmail = ConfigurationManager.AppSettings["MailFrom"];

                // Log configuration (without sensitive data)
                System.Diagnostics.Debug.WriteLine($"Email Service - Starting email send");
                System.Diagnostics.Debug.WriteLine($"  SmtpHost: {smtpHost}");
                System.Diagnostics.Debug.WriteLine($"  SmtpPort: {smtpPort}");
                System.Diagnostics.Debug.WriteLine($"  SmtpUser: {(string.IsNullOrWhiteSpace(smtpUser) ? "NULL" : "SET")}");
                System.Diagnostics.Debug.WriteLine($"  SmtpPass: {(string.IsNullOrWhiteSpace(smtpPass) ? "NULL" : "SET")}");
                System.Diagnostics.Debug.WriteLine($"  MailFrom: {fromEmail}");
                System.Diagnostics.Debug.WriteLine($"  ToEmail: {toEmail}");

                // Validate configuration
                if (string.IsNullOrWhiteSpace(smtpUser))
                {
                    throw new Exception("SmtpUser is not configured in Web.config");
                }
                if (string.IsNullOrWhiteSpace(smtpPass))
                {
                    throw new Exception("SmtpPass is not configured in Web.config");
                }
                if (string.IsNullOrWhiteSpace(fromEmail))
                {
                    throw new Exception("MailFrom is not configured in Web.config");
                }

                System.Diagnostics.Debug.WriteLine($"Configuration validated successfully");

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    System.Diagnostics.Debug.WriteLine($"Creating SMTP client connection...");

                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;
                    client.Timeout = 10000; // 10 seconds timeout

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(fromEmail, "LearnLink");
                        mailMessage.To.Add(new MailAddress(toEmail));
                        mailMessage.Subject = "Verify Your LearnLink Account";
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = $@"
                            <html>
                                <head>
                                    <style>
                                        body {{ font-family: Arial, sans-serif; color: #333; }}
                                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                        .header {{ background-color: #4CAF50; color: white; padding: 20px; border-radius: 5px; text-align: center; }}
                                        .content {{ padding: 20px; border: 1px solid #ddd; margin-top: 10px; }}
                                        .button {{ display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                                        .footer {{ margin-top: 20px; font-size: 12px; color: #888; text-align: center; }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>
                                            <h2>Welcome to LearnLink!</h2>
                                        </div>
                                        <div class='content'>
                                            <p>Thank you for registering with LearnLink. To complete your registration and activate your account, please verify your email by clicking the button below:</p>
                                            <p><a href='{verificationLink}' class='button'>Verify Email Address</a></p>
                                            <p>Or copy and paste this link in your browser:</p>
                                            <p><small>{verificationLink}</small></p>
                                            <p>This link will expire in 24 hours.</p>
                                        </div>
                                        <div class='footer'>
                                            <p>If you did not create this account, please ignore this email.</p>
                                            <p>&copy; 2024 LearnLink. All rights reserved.</p>
                                        </div>
                                    </div>
                                </body>
                            </html>";

                        System.Diagnostics.Debug.WriteLine($"Sending email to: {toEmail}");
                        await client.SendMailAsync(mailMessage);
                        System.Diagnostics.Debug.WriteLine($"Email sent successfully to: {toEmail}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}