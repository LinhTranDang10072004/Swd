using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace FinalProject.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var host = _config["Email:Smtp:Host"];
        var portStr = _config["Email:Smtp:Port"];
        var user = _config["Email:Smtp:Username"];
        var pass = _config["Email:Smtp:Password"];
        var from = _config["Email:Smtp:From"] ?? user;
        var enableSslStr = _config["Email:Smtp:EnableSsl"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            return;
        }

        _ = int.TryParse(portStr, out var port);
        if (port <= 0) port = 587;
        var enableSsl = true;
        if (bool.TryParse(enableSslStr, out var parsedSsl))
        {
            enableSsl = parsedSsl;
        }

        using var msg = new MailMessage(from, toEmail)
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        if (!string.IsNullOrWhiteSpace(user))
        {
            client.Credentials = new NetworkCredential(user, pass);
        }

        await client.SendMailAsync(msg);
    }
}

