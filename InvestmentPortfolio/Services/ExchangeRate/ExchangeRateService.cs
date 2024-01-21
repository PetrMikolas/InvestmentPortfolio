using InvestmentPortfolio.Models;
using InvestmentPortfolio.Services.Email;
using System.Globalization;

namespace InvestmentPortfolio.Services.ExchangeRate;

public class ExchangeRateService(HttpClient httpClient, IConfiguration configuration, IEmailService email, ILogger<ExchangeRateService> logger) : IExchangeRateService
{
    public async Task<ExchangeRates?> GetExchangeRatesAsync(CancellationToken cancellationToken)
    {
        Stream data;
        var url = configuration["UrlApiCnb"];

        if (string.IsNullOrEmpty(url))
        {
            logger.LogError("Nelze načíst URL API ČNB");
            email.SendError("Nelze načíst URL API ČNB", typeof(ExchangeRateService), nameof(GetExchangeRatesAsync));               
            return null;
        }

        try
        {
            data = await httpClient.GetStreamAsync(url, cancellationToken);
        }
        catch
        {
            logger.LogError("Nepodařilo se připojit k API ČNB");
            email.SendError("Nepodařilo se připojit k API ČNB", typeof(ExchangeRateService), nameof(GetExchangeRatesAsync));
            return null;            
        }

        return ReadExchangeRates(data);
    }

    private ExchangeRates? ReadExchangeRates(Stream stream)
    {
        // NumberDecimalSeparator musí být nastaven kvůli nasazení v Dockeru na Linuxu - parsování float
        var cultureInfo = new CultureInfo("") { NumberFormat = new NumberFormatInfo() { NumberDecimalSeparator = "," } };
        
        try
        {
            using var reader = new StreamReader(stream);

            // načte 1. řádek tabulky obsahující datum
            var row = reader.ReadLine() ?? throw new ArgumentNullException(nameof(reader), "Nepodařilo se načíst směnné kurzy");

            var exchangeRates = new ExchangeRates
            {
                Date = row.Split('#').First().Trim()
            };

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
                    Rate = float.Parse(values[4], cultureInfo)
                });
            }

            return exchangeRates;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            email.SendError(ex.ToString());
            return null;
        }
    }
}