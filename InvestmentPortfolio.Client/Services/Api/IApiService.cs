namespace InvestmentPortfolio.Client.Services.Api;

public interface IApiService
{
    Task<InvestmentsDto> GetInvestmentsAsync(bool isRefresh = false, CancellationToken cancellationToken = default);

    Task SaveInvestmentAsync(InvestmentDto investment, CancellationToken cancellationToken = default);

    Task UpdateInvestmentAsync(InvestmentDto investment, CancellationToken cancellationToken = default);

    Task DeleteInvestmentAsync(int id, CancellationToken cancellationToken = default);
}