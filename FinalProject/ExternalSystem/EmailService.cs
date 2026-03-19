using System.Net;
using System.Net.Mail;

namespace FinalProject.ExternalSystem
{
    public class EmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = "xumlmhe186012@fpt.edu.vn";
            var appPassword = "ebxeblkmhzrsteni";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
