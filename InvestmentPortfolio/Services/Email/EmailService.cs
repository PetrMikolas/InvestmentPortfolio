using MailKit.Net.Smtp;
using MimeKit;
using System.Text.Json;

namespace InvestmentPortfolio.Services.Email;

public sealed class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public void SendError(string errorMessage)
    {
        SendErrorMessage(errorMessage);
    }

    public void SendError(string errorMessage, Type typeClass, string nameMethod)
    {
        SendErrorMessage(errorMessage, typeClass, nameMethod);
    }

    private void SendErrorMessage(string errorMessage, Type? typeClass = null, string nameMethod = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(errorMessage);

        if ((typeClass is not null && string.IsNullOrEmpty(nameMethod)) || (typeClass is null && !string.IsNullOrEmpty(nameMethod)))
            throw new ArgumentException("Nejsou vyplněny všechny argumenty metody SendError");

        string message = $"Error: {errorMessage}";

        if (typeClass is not null && !string.IsNullOrEmpty(nameMethod))
            message = $"{message}\n   at {typeClass}.{nameMethod}()";

        SendEmail(message, "Investment portfolio - error");
    }

    public void SendObject<TValue>(TValue value, string subject) where TValue : class
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(subject);

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonValue = JsonSerializer.Serialize(value, options);
        SendEmail(jsonValue, subject);
    }

    private void SendEmail(string message, string subject)
    {
        var smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

        if (smtpSettings is null)
        {
            logger.LogError("Nelze načíst nastavení smtp klienta");
            return;
        }

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress("Investment portfolio", smtpSettings.EmailAddress));
        mimeMessage.To.Add(new MailboxAddress(string.Empty, smtpSettings.EmailAddress));
        mimeMessage.Subject = subject ?? "Zpráva z aplikace Investment portfolio";
        mimeMessage.Body = new TextPart("plain") { Text = message };

        try
        {
            using var smtpClient = new SmtpClient();
            smtpClient.Connect(smtpSettings.Host, smtpSettings.Port, smtpSettings.UseSsl);
            smtpClient.Authenticate(smtpSettings.UserName, smtpSettings.Password);
            smtpClient.Send(mimeMessage);
            smtpClient.Disconnect(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }

    private sealed record SmtpSettings
    (
         string Host,
         int Port,
         string UserName,
         string Password,
         string EmailAddress,
         bool UseSsl
    );
}