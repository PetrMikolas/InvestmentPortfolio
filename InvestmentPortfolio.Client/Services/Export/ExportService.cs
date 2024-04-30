using InvestmentPortfolio.Client.Services.Api;
using Microsoft.JSInterop;
using System.Globalization;
using System.Text;

namespace InvestmentPortfolio.Client.Services.Export;

/// <summary>
/// Service for exporting investment data to a CSV file.
/// Implements the <see cref="IExportService"/> interface.
/// </summary>
/// <param name="JSRuntime">The JavaScript runtime instance.</param>
/// <param name="apiClient">The API client instance.</param>
public sealed class ExportService(IJSRuntime JSRuntime, IApiClient apiClient) : IExportService
{   
    /// <summary>
    /// Downloads investment data in CSV format asynchronously.
    /// </summary>
    /// <param name="investments">The list of investments to export.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when there is an issue with downloading the CSV file.</exception>
    public async Task DownloadInvestmentsCsvFile(List<Investment> investments)
    {
        IJSObjectReference JSmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts.js");

        string fileName = $"Investice_{DateTime.Now.ToShortDateString()}.csv";
        byte[] buffer = Encoding.UTF8.GetBytes(GetContentFile(investments));
        var fileStream = new MemoryStream(buffer);
        using var streamReference = new DotNetStreamReference(stream: fileStream);

        try
        {
            await JSmodule.InvokeVoidAsync("downloadFileFromStream", fileName, streamReference);
        }
        catch (Exception ex)
        {
            await apiClient.SendErrorAsync(ex.ToString());
            throw new Exception("CSV soubor nelze stáhnout. O problému víme a pracujeme na nápravě.");
        }
    }

    /// <summary>
    /// Generates the content for the CSV file.
    /// </summary>
    /// <param name="investments">The list of investments.</param>
    /// <returns>The content of the CSV file as a string.</returns>
    private string GetContentFile(List<Investment> investments)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("Nazev;Hodnota;Mena;Hodnota Kc;Podil %");

        foreach (var investment in investments)
        {
            stringBuilder.AppendLine($"{RemoveDiacritics(investment.Name)};{investment.Value};{investment.CurrencyCode};{investment.ValueCzk};{investment.PercentageShare}");
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Removes diacritics from the specified text.
    /// </summary>
    /// <param name="text">The text from which to remove diacritics.</param>
    /// <returns>The text with diacritics removed.</returns>
    private string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        foreach (char item in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(item);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(item);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}