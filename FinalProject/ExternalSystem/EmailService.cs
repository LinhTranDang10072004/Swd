using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SWD392.ExternalSystem;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var fromEmail = _config["Email:Gmail:FromEmail"];
        var appPassword = _config["Email:Gmail:AppPassword"];

        if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(appPassword))
        {
            return;
        }

        using var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromEmail, appPassword),
            EnableSsl = true
        };

        using var mailMessage = new MailMessage
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

