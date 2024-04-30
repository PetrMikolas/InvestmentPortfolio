using MimeKit.Text;

namespace InvestmentPortfolio.Services.Email;

/// <summary>
/// Service for sending emails. 
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an error message asynchronously.
    /// </summary>
    /// <param name="errorMessage">The error message to be sent.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendErrorAsync(string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an error message asynchronously, including information about the class and method where the error occurred.
    /// </summary>
    /// <param name="errorMessage">The error message to be sent.</param>
    /// <param name="typeClass">The type of the class where the error occurred.</param>
    /// <param name="nameMethod">The name of the method where the error occurred.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendErrorAsync(string errorMessage, Type typeClass, string nameMethod, CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes an object to JSON and sends it asynchronously via email.
    /// </summary>
    /// <typeparam name="TValue">The type of the object to be serialized.</typeparam>
    /// <param name="value">The object to be serialized and sent.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendObjectAsync<TValue>(TValue value, string subject, CancellationToken cancellationToken = default) where TValue : class;

    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="message">The message content of the email.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="address">The recipient email address.</param>
    /// <param name="name">The recipient name (optional). Defaults to an empty string.</param>
    /// <param name="textFormat">The format of the email content (optional). Defaults to <see cref="TextFormat.Plain"/>.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(string message, string subject, string address, string name = "", TextFormat textFormat = TextFormat.Plain, CancellationToken cancellationToken = default);
}