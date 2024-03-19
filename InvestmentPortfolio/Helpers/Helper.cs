using InvestmentPortfolio.Models;

namespace InvestmentPortfolio.Helpers;

public static class Helper
{
    public static bool IsValidInvestmentDto(InvestmentDto? investmentDto, HttpMethod httpMethod, out string error)
    {
        error = string.Empty;

        if (investmentDto is null)
        {
            error = $"Parametr <{nameof(investmentDto)}> nemůže být null.";
            return false;
        }

        if (httpMethod == HttpMethod.Post && investmentDto.Id != 0)
        {
            error = $"Parametr <{nameof(investmentDto.Id)}> musí mít hodnotu 0. ";
        }

        if (httpMethod != HttpMethod.Post && investmentDto.Id <= 0)
        {
            error = $"Parametr <{nameof(investmentDto.Id)}> musí mít hodnotu větší než 0. ";
        }

        if (investmentDto.Name is null)
        {
            error += $"Parametr <{nameof(investmentDto.Name)}> nemůže být null. ";
        }
        else if (investmentDto.Name.Length <= 0 || investmentDto.Name.Length > 40)
        {
            error += $"Parametr <{nameof(investmentDto.Name)}> musí mít délku 1 až 40 znaků. ";
        }

        if (investmentDto.CurrencyCode is null)
        {
            error += $"Parametr <{nameof(investmentDto.CurrencyCode)}> nemůže být null. ";
        }
        else if (investmentDto.CurrencyCode.Length != 3)
        {
            error += $"Parametr <{nameof(investmentDto.CurrencyCode)}> musí mít délku 3 znaky. ";
        }

        if (investmentDto.Value <= 0 || investmentDto.Value > 100000000)
        {
            error += $"Parametr <{nameof(investmentDto.Value)}> musí mít hodnotu v rozsahu 1 až 100 000 000.";
        }

        return error == string.Empty;
    }

    public static bool ParseBoolEnvironmentVariable(string environmentVariableName, bool defaultValue = false)
    {
        ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(ParseBoolEnvironmentVariable));
        var value = Environment.GetEnvironmentVariable(environmentVariableName);

        if (string.IsNullOrEmpty(value))
        {
            logger.LogWarning($"Hodnota proměnné prostředí {environmentVariableName} není nastavena. Použita výchozí hodnota: {defaultValue}");
            return defaultValue;
        }

        if (!bool.TryParse(value, out var result))
        {
            logger.LogWarning($"Hodnota proměnné prostředí {environmentVariableName} ({value}) není platná boolean hodnota. Použita výchozí hodnota: {defaultValue}");
            result = defaultValue;
        }

        return result;
    }
}