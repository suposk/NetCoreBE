//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using MimeKit.Text;
//using System.Threading.Tasks;

namespace CommonCleanArch.Infrastructure;
public record SentEmailDto(string From, string To, string Subject, string Text, bool IsHtml);

public class EmailConfig
{
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; }

    public required string SmtpUser { get; set; }

    public required string SmtpPass { get; set; }

    public bool HasPassword => !string.IsNullOrWhiteSpace(SmtpPass);
}

public interface IEmailService
{
    Task SendEmail(string from, string to, string subject, string text, bool isHtml);
    Task<bool> SendEmail(SentEmailDto sentEmail);
    Task SendEmail(string from, List<string> toList, string subject, string text, bool isHtml);
}

public class EmailService : IEmailService
{
    //private readonly EmailConfig _emailConfig;
    //public EmailService(IConfiguration configuration)
    //{
    //    _emailConfig = configuration.GetSection(nameof(EmailConfig)).Get<EmailConfig>();
    //}

    public async Task<bool> SendEmail(SentEmailDto sentEmail)
    {        
        return true;
    }

    public async Task SendEmail(string from, string to, string subject, string text, bool isHtml)
    {
        return;
    }

    public async Task SendEmail(string from, List<string> toList, string subject, string text, bool isHtml)
    {
        return;
    }
}