using InvestmentPortfolio.Models;

namespace InvestmentPortfolio.Services.ExchangeRate;

/// <summary>
/// Interface for retrieving exchange rates from the ČNB (Czech National Bank) API.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Asynchronously retrieves exchange rates from the ČNB API.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>Returns exchange rates.</returns>
    Task<ExchangeRates> GetExchangeRatesAsync(CancellationToken cancellationToken = default);
}