using InvestmentPortfolio.Client.Services.Api;
using Microsoft.JSInterop;
using System.Globalization;
using System.Text;

namespace InvestmentPortfolio.Client.Services.Export;

public sealed class ExportService(IJSRuntime JSRuntime, IApiClient apiClient) : IExportService
{
    private IJSObjectReference? _JSmodule;

    public async Task DownloadInvestmentsCsvFile(List<Investment> investments)
    {
        _JSmodule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts.js");

        string fileName = $"Investice_{DateTime.Now.ToShortDateString()}.csv";
        byte[] buffer = Encoding.UTF8.GetBytes(GetContentFile(investments));
        var fileStream = new MemoryStream(buffer);
        using var streamReference = new DotNetStreamReference(stream: fileStream);

        try
        {
            await _JSmodule.InvokeVoidAsync("downloadFileFromStream", fileName, streamReference);
        }
        catch (Exception ex)
        {
            await apiClient.SendErrorAsync(ex.ToString());
            throw new Exception("CSV soubor nelze stáhnout. O problému víme a pracujeme na nápravě.");
        }
    }

    private string GetContentFile(List<Investment> investments)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Nazev;Hodnota;Mena;Hodnota Kc;Podil %");

        foreach (var investment in investments)
        {
            sb.AppendLine($"{RemoveDiacritics(investment.Name)};{investment.Value};{investment.CurrencyCode};{investment.ValueCzk};{investment.Percentage.Replace("%", "")}");
        }

        return sb.ToString();
    }

    public string RemoveDiacritics(string text)
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