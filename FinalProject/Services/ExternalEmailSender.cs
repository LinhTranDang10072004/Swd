using FinalProject.Services;
using SWD392.ExternalSystem;

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

