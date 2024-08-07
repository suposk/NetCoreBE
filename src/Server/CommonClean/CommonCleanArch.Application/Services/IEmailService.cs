namespace CommonCleanArch.Application.Services;

public record SentEmailDto(string From, string To, string Subject, string Text, bool IsHtml);
public interface IEmailService
{
    Task SendEmail(string from, string to, string subject, string text, bool isHtml);
    Task<bool> SendEmail(SentEmailDto sentEmail);
    Task SendEmail(string from, List<string> toList, string subject, string text, bool isHtml);
}
