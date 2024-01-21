using System.Text.Json;

namespace InvestmentPortfolio.IntegrationTests;

internal static class Constants
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
}