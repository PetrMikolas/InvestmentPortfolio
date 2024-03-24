using InvestmentPortfolio.Models;
using InvestmentPortfolio.Services.Email;

namespace InvestmentPortfolio.Services.ExchangeRate;

public class ExchangeRateService(HttpClient httpClient, IConfiguration configuration, IEmailService email, ILogger<ExchangeRateService> logger) : IExchangeRateService
{
    public async Task<ExchangeRates> GetExchangeRatesAsync(CancellationToken cancellationToken)
    {
        var url = configuration["UrlApiCnb"];
        var exchangeRates = new ExchangeRates();

        if (string.IsNullOrEmpty(url))
        {
            logger.LogError("Nelze načíst URL API ČNB");
            _ = email.SendErrorAsync("Nelze načíst URL API ČNB", typeof(ExchangeRateService), nameof(GetExchangeRatesAsync), cancellationToken);
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
            _ = email.SendErrorAsync(ex.ToString(), typeof(ExchangeRateService), nameof(GetExchangeRatesAsync), cancellationToken);
        }

        return exchangeRates;
    }

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