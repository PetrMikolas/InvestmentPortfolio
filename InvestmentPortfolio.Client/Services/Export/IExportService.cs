using InvestmentPortfolio.Client.Services.Api;

namespace InvestmentPortfolio.Client.Services.Export;

public interface IExportService
{
    public Task DownloadInvestmentsCsvFile(List<Investment> investments);
}