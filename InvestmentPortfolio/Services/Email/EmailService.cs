using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace InvestmentPortfolio.Services.Email;

/// <summary>
/// Service for managing email-related operations, based on the <seealso cref="IEmailService"/> interface.
/// </summary>
internal sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="options">The email options.</param>
    /// <param name="logger">The logger instance for logging errors and information.</param>
    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _options = InitializeOptions(options);
    }

    /// <summary>
    /// Initializes the email options from the provided configuration, handling any validation exceptions.
    /// </summary>
    /// <param name="options">The email configuration options.</param>
    /// <returns>The initialized email options.</returns>
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

    /// <summary>
    /// Sends an error message asynchronously, optionally including information about the class and method where the error occurred.
    /// </summary>
    /// <param name="errorMessage">The error message to be sent.</param>
    /// <param name="typeClass">The type of the class where the error occurred (optional).</param>
    /// <param name="nameMethod">The name of the method where the error occurred (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <exception cref="ArgumentException">Thrown when not all required method arguments are provided.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Represents the configuration options for the email service.
    /// </summary>
    public sealed record EmailOptions
    {
        /// <summary>
        /// The key for accessing the email options.
        /// </summary>
        public const string Key = "EmailOptions";

        /// <summary>
        /// Gets or sets the SMTP host address.
        /// </summary>
        [Required]
        public string SmtpHost { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP port number.
        /// </summary>
        [Required]
        public int SmtpPort { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL/TLS should be used for SMTP.
        /// </summary>
        [Required]
        public bool SmtpUseSsl { get; init; }

        /// <summary>
        /// Gets or sets the SMTP username.
        /// </summary>
        [Required]
        public string SmtpUserName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP password.
        /// </summary>
        [Required]
        public string SmtpPassword { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the name displayed as the sender of the email.
        /// </summary>
        [Required]
        public string FromName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address used as the sender.
        /// </summary>
        [Required]
        public string FromEmailAddress { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address where error notifications are sent.
        /// </summary>
        [Required]
        public string AdminEmailAddress { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the default subject for emails.
        /// </summary>
        public string DefaultSubject { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the name displayed as the BCC recipient of the email.
        /// </summary>
        public string BccName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address used for BCC recipients.
        /// </summary>
        public string BccEmailAddress { get; init; } = string.Empty;
    }
}