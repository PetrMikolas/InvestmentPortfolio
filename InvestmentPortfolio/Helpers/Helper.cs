using System.Runtime.CompilerServices;

namespace InvestmentPortfolio.Helpers;

/// <summary>
/// Helper methods for investment-related operations.
/// </summary>
public static class Helper
{
    /// <summary>
    /// Parses a boolean environment variable with the specified name.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="environmentVariableName">The name of the environment variable to parse.</param>
    /// <param name="defaultValue">The default value to return if the environment variable is not set or cannot be parsed.</param>
    /// <returns>The boolean value of the environment variable if successfully parsed; otherwise, the default value.</returns>
    public static bool ParseBoolEnvironmentVariable(this WebApplication app, string environmentVariableName, bool defaultValue = false)
    {
        var logger = app.CreateAutoLogger();
        var value = Environment.GetEnvironmentVariable(environmentVariableName);

        if (string.IsNullOrEmpty(value))
        {
            logger.LogInformation($"Hodnota proměnné prostředí {environmentVariableName} není nastavena. Použita výchozí hodnota: {defaultValue}");
            return defaultValue;
        }

        if (!bool.TryParse(value, out var result))
        {
            logger.LogWarning($"Hodnota proměnné prostředí {environmentVariableName} ({value}) není platná boolean hodnota. Použita výchozí hodnota: {defaultValue}");
            result = defaultValue;
        }

        return result;
    }

    /// <summary>
    /// Vytvoří logger s kategorií automaticky odvozenou z volajícího souboru,
    /// pokud není explicitně předána.
    /// </summary>
    public static ILogger CreateAutoLogger(this IServiceCollection services, [CallerFilePath] string callerFilePath = "")
    {
        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ILoggerFactory>();

        var categoryName = GetCategoryNameFromPath(callerFilePath);

        return factory.CreateLogger(categoryName);
    }

    /// <summary>
    /// Vytvoří logger z <see cref="WebApplication"/> s kategorií odvozenou z volajícího souboru.
    /// </summary>
    public static ILogger CreateAutoLogger(this WebApplication app, [CallerFilePath] string callerFilePath = "")
    {
        var factory = app.Services.GetRequiredService<ILoggerFactory>();
        var categoryName = Path.GetFileNameWithoutExtension(callerFilePath);

        return factory.CreateLogger(categoryName);
    }

    private static string GetCategoryNameFromPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "Unknown";

        return Path.GetFileNameWithoutExtension(path);
    }
}