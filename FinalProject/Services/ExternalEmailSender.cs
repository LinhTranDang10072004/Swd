using FinalProject.ExternalSystem;
using FinalProject.Services;

namespace FinalProject.Services;

public class ExternalEmailSender : IEmailSender
{
    private readonly EmailService _emailService;

    public ExternalEmailSender(EmailService emailService)
    {
        _emailService = emailService;
    }

    public Task SendAsync(string toEmail, string subject, string htmlBody)
        => _emailService.SendEmailAsync(toEmail, subject, htmlBody);
}

