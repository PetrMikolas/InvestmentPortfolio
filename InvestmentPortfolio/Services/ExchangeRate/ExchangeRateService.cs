using InvestmentPortfolio.Models;
using InvestmentPortfolio.Services.Email;

namespace InvestmentPortfolio.Services.ExchangeRate;

/// <summary>
/// Service for retrieving exchange rates from the ČNB (Czech National Bank) API, implementing the <see cref="IExchangeRateService"/> interface.
/// </summary>
/// <param name="httpClient">The HTTP client used for making requests to the ČNB API.</param>
/// <param name="configuration">The configuration of the application.</param>
/// <param name="email">The service for sending email notifications.</param>
/// <param name="logger">The logger instance for logging.</param>
internal sealed class ExchangeRateService(HttpClient httpClient, IConfiguration configuration, IEmailService email, ILogger<ExchangeRateService> logger) : IExchangeRateService
{
    public async Task<ExchangeRates> GetAsync(CancellationToken cancellationToken)
    {
        var url = configuration["UrlApiCnb"];
        var exchangeRates = new ExchangeRates();

        if (string.IsNullOrEmpty(url))
        {
            logger.LogError("Nelze načíst URL API ČNB");
            _ = email.SendErrorAsync("Nelze načíst URL API ČNB", typeof(ExchangeRateService), nameof(GetAsync), cancellationToken);
            return exchangeRates;
        }

        try
        {
            var data = await httpClient.GetStreamAsync(url, cancellationToken);
            exchangeRates = ReadExchangeRates(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Nepodařilo se připojit k API ČNB");
            _ = email.SendErrorAsync(ex.ToString(), typeof(ExchangeRateService), nameof(GetAsync), cancellationToken);
        }

        return exchangeRates;
    }

    /// <summary>
    /// Reads exchange rates from the provided stream.
    /// </summary>
    /// <param name="stream">The stream containing exchange rate data.</param>
    /// <returns>Returns exchange rates.</returns>
    private ExchangeRates ReadExchangeRates(Stream stream)
    {
        var exchangeRates = new ExchangeRates();

        try
        {
            using var reader = new StreamReader(stream);

            // načte 1. řádek tabulky obsahující datum
            var row = reader.ReadLine() ?? throw new ArgumentNullException(nameof(reader), "Nepodařilo se načíst směnné kurzy");
            exchangeRates.Date = row.Split('#').First().Trim();

            // načte 2. řádek tabulky se kterým nepracujeme
            row = reader.ReadLine();

            // načte ostatní řádky tabulky, které obsahují devizové kurzy
            while ((row = reader.ReadLine()) != null)
            {
                var values = row.Split('|');

                exchangeRates.Items.Add(new Models.ExchangeRate()
                {
                    Country = values[0],
                    Currency = values[1],
                    Amount = int.Parse(values[2]),
                    Code = values[3],
                    Rate = float.Parse(values[4])
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            _ = email.SendErrorAsync(ex.ToString());
        }

        return exchangeRates;
    }
}