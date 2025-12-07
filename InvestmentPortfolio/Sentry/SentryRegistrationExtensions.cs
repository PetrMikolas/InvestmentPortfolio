using InvestmentPortfolio.Helpers;

namespace InvestmentPortfolio.Sentry;

public static class SentryRegistrationExtensions
{
    /// <summary>
    /// Adds Sentry services to the <see cref="IWebHostBuilder"/> using the provided configuration.
    /// </summary>
    /// <param name="webHostBuilder">The <see cref="IWebHostBuilder"/> instance to which Sentry services will be added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to retrieve the Sentry DSN and other settings.</param>
    /// <returns>The <see cref="IWebHostBuilder"/> instance with Sentry configured.</returns>
    public static IWebHostBuilder AddSentry(this IWebHostBuilder webHostBuilder, IConfiguration configuration)
    {
        var dsn = configuration["SentryDsn"];

        if (string.IsNullOrWhiteSpace(dsn))
        {
            webHostBuilder.ConfigureLogging(logging =>
            {
                using var provider = logging.Services.BuildServiceProvider();

                provider.CreateLogger().LogWarning(
                    "The Sentry DSN is missing or empty. Without a valid DSN, Sentry will not be operational. " +
                    "Ensure that the DSN is specified in the appsettings.json file under the key 'SentryDsn'."
                );
            });

            return webHostBuilder;
        }

        return webHostBuilder.UseSentry(options =>
        {
            options.Dsn = dsn;
            options.Debug = false;
            options.TracesSampleRate = 1.0;
        });
    }
}