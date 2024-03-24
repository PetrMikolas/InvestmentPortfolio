using InvestmentPortfolio.Models;

namespace InvestmentPortfolio.Services.ExchangeRate;

public interface IExchangeRateService
{
    public Task<ExchangeRates> GetExchangeRatesAsync(CancellationToken cancellationToken);
}