using MimeKit.Text;

namespace InvestmentPortfolio.Services.Email;

public interface IEmailService
{
    Task SendErrorAsync(string errorMessage, CancellationToken cancellationToken = default);

    Task SendErrorAsync(string errorMessage, Type typeClass, string nameMethod, CancellationToken cancellationToken = default);

    Task SendObjectAsync<TValue>(TValue value, string subject, CancellationToken cancellationToken = default) where TValue : class;

    Task SendEmailAsync(string message, string subject, string address, string name = "", TextFormat textFormat = TextFormat.Plain, CancellationToken cancellationToken = default);
}