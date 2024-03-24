using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace InvestmentPortfolio.Services.Email;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _options = InitializeOptions(options);
    }

    private EmailOptions InitializeOptions(IOptions<EmailOptions> options)
    {
        try
        {
            return options.Value;
        }
        catch (OptionsValidationException ex)
        {
            _logger.LogError(ex, $"Chyba při validaci {nameof(EmailOptions)} pro EmailService");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Chyba při inicializaci {nameof(EmailOptions)} pro EmailService");
        }

        return new EmailOptions();
    }

    public async Task SendErrorAsync(string errorMessage, CancellationToken cancellationToken = default)
    {
        await SendErrorMessageAsync(errorMessage, cancellationToken: cancellationToken);
    }

    public async Task SendErrorAsync(string errorMessage, Type typeClass, string nameMethod, CancellationToken cancellationToken = default)
    {
        await SendErrorMessageAsync(errorMessage, typeClass, nameMethod, cancellationToken: cancellationToken);
    }

    private async Task SendErrorMessageAsync(string errorMessage, Type? typeClass = null, string nameMethod = "", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorMessage, nameof(errorMessage));

        if ((typeClass is not null && string.IsNullOrEmpty(nameMethod)) || (typeClass is null && !string.IsNullOrEmpty(nameMethod)))
            throw new ArgumentException("Nejsou vyplněny všechny argumenty metody SendError");

        string message = $"Error: {errorMessage}";

        if (typeClass is not null && !string.IsNullOrEmpty(nameMethod))
            message = $"{message}\n   at {typeClass}.{nameMethod}()";

        await SendEmailAsync(message, $"{_options.FromName} - error", _options.AdminEmailAddress, cancellationToken: cancellationToken);
    }

    public async Task SendObjectAsync<TValue>(TValue value, string subject, CancellationToken cancellationToken = default) where TValue : class
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonValue = JsonSerializer.Serialize(value, options);
        await SendEmailAsync(jsonValue, subject, _options.AdminEmailAddress, cancellationToken: cancellationToken);
    }

    public async Task SendEmailAsync(string message, string subject, string address, string name = "", TextFormat textFormat = TextFormat.Plain, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(address, nameof(address));

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromEmailAddress));
        mimeMessage.To.Add(new MailboxAddress(name, address));
        mimeMessage.Subject = !string.IsNullOrEmpty(subject) ? subject : _options.DefaultSubject;
        mimeMessage.Body = new TextPart(textFormat) { Text = message ?? string.Empty };

        if (!string.IsNullOrEmpty(_options.BccEmailAddress))
        {
            mimeMessage.Bcc.Add(new MailboxAddress(_options.BccName, _options.BccEmailAddress));
        }

        try
        {
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_options.SmtpHost, _options.SmtpPort, _options.SmtpUseSsl, cancellationToken);
            await smtpClient.AuthenticateAsync(_options.SmtpUserName, _options.SmtpPassword, cancellationToken);
            await smtpClient.SendAsync(mimeMessage, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    public sealed record EmailOptions
    {
        public const string Key = "EmailOptions";

        [Required]
        public string SmtpHost { get; init; } = string.Empty;

        [Required]
        public int SmtpPort { get; init; }

        [Required]
        public bool SmtpUseSsl { get; init; }

        [Required]
        public string SmtpUserName { get; init; } = string.Empty;

        [Required]
        public string SmtpPassword { get; init; } = string.Empty;

        [Required]
        public string FromName { get; init; } = string.Empty;

        [Required]
        public string FromEmailAddress { get; init; } = string.Empty;

        [Required]
        public string AdminEmailAddress { get; init; } = string.Empty;

        public string DefaultSubject { get; init; } = string.Empty;

        public string BccName { get; init; } = string.Empty;

        public string BccEmailAddress { get; init; } = string.Empty;
    }
}