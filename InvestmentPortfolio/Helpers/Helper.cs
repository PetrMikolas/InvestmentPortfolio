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
        var logger = app.CreateLogger();
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
    /// Creates an <see cref="ILogger"/> using the specified <see cref="IServiceProvider"/>, generating the logger category automatically from the calling class and member name.
    /// </summary>
    /// <remarks>
    /// The logger category is constructed in the form "<c>{ClassName}.{MemberName}</c>", where <c>ClassName</c> is derived from <paramref name="callerFilePath"/> and
    /// <c>MemberName</c> is derived from <paramref name="callerMemberName"/>. Both parameters are supplied automatically by the compiler.
    /// </remarks>
    /// <param name="provider"> The <see cref="IServiceProvider"/> used to resolve the <see cref="ILoggerFactory"/>.</param>
    /// <param name="callerFilePath">Automatically supplied path of the source file containing the caller.</param>
    /// <param name="callerMemberName">Automatically supplied name of the calling method or property.</param>
    /// <returns>An <see cref="ILogger"/> instance with a category name derived from the calling file and member.</returns>
    public static ILogger CreateLogger(this IServiceProvider provider, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        var factory = provider.GetRequiredService<ILoggerFactory>();
        var categoryName = GetCategoryName(callerFilePath, callerMemberName);

        return factory.CreateLogger(categoryName);
    }

    /// <summary>
    /// Creates an <see cref="ILogger"/> for the specified <see cref="WebApplication"/>, using the calling file and member to determine the logger category.
    /// </summary>
    /// <remarks>
    /// The logger category is constructed as "<c>{ClassName}.{MemberName}</c>", where <c>ClassName</c> is derived from the calling file name and
    /// <c>MemberName</c> is the name of the calling method or property. Both values are supplied automatically by the compiler.
    /// </remarks>
    /// <param name="app">The <see cref="WebApplication"/> from whose services the logger factory is resolved.</param>
    /// <param name="callerFilePath">Automatically supplied path of the source file containing the caller.</param>
    /// <param name="callerMemberName">Automatically supplied name of the calling method or property.</param>
    /// <returns>An <see cref="ILogger"/> instance with a category name derived from the calling file and member.</returns>
    public static ILogger CreateLogger(this WebApplication app, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        var factory = app.Services.GetRequiredService<ILoggerFactory>();
        var categoryName = GetCategoryName(callerFilePath, callerMemberName);

        return factory.CreateLogger(categoryName);
    }        

    private static string GetCategoryName(string callerFilePath, string callerMemberName)
    {
        var className = Path.GetFileNameWithoutExtension(callerFilePath);
        return $"{className}.{callerMemberName}";
    }
}