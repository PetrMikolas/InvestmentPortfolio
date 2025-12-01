namespace InvestmentPortfolio.Helpers;

/// <summary>
/// Helper methods for investment-related operations.
/// </summary>
public static class Helper
{
    private static readonly ILogger _logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger(typeof(Helper));

    /// <summary>
    /// Parses a boolean environment variable with the specified name.
    /// </summary>
    /// <param name="environmentVariableName">The name of the environment variable to parse.</param>
    /// <param name="defaultValue">The default value to return if the environment variable is not set or cannot be parsed.</param>
    /// <returns>The boolean value of the environment variable if successfully parsed; otherwise, the default value.</returns>
    public static bool ParseBoolEnvironmentVariable(string environmentVariableName, bool defaultValue = false)
    {        
        var value = Environment.GetEnvironmentVariable(environmentVariableName);

        if (string.IsNullOrEmpty(value))
        {
            _logger.LogInformation($"Hodnota proměnné prostředí {environmentVariableName} není nastavena. Použita výchozí hodnota: {defaultValue}");
            return defaultValue;
        }

        if (!bool.TryParse(value, out var result))
        {
            _logger.LogWarning($"Hodnota proměnné prostředí {environmentVariableName} ({value}) není platná boolean hodnota. Použita výchozí hodnota: {defaultValue}");
            result = defaultValue;
        }

        return result;
    }
}