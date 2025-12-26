using MimeKit.Text;
using System.Runtime.CompilerServices;

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
    /// Sends an error message along with contextual information about the caller's source file and member name.
    /// </summary>
    /// <param name="errorMessage">The error message to be sent.</param>
    /// <param name="callerFilePath">Automatically supplied path of the source file containing the caller.</param>
    /// <param name="callerMemberName">Automatically supplied name of the calling method or property.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendErrorWithContextAsync(string errorMessage, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes an object to JSON and sends it asynchronously via email.
    /// </summary>
    /// <typeparam name="TValue">The type of the object to be serialized.</typeparam>
    /// <param name="value">The object to be serialized and sent.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendObjectAsync<TValue>(TValue value, string subject, CancellationToken cancellationToken = default) where TValue : class;    
}