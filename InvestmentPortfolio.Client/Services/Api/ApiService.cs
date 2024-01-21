using InvestmentPortfolio.Client.Exceptions;

namespace InvestmentPortfolio.Client.Services.Api;

public sealed class ApiService(IApiClient apiClient) : IApiService
{
    public async Task<InvestmentsDto> GetInvestmentsAsync(bool hasRefresExchangeRates = false, CancellationToken cancellationToken = default)
    {
        try
        {
            return await apiClient.GetInvestmentsAsync(hasRefresExchangeRates, cancellationToken);
        }
        catch (ApiException ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new ApiInvestmentsResponseException("Nepodařilo se načíst investice. Zkuste to prosím později.");
        }
        catch (HttpRequestException)
        {
            throw new HttpRequestException("Nelze se připojit k internetu. Zkontrolujte prosím připojení a zkuste to znovu.");
        }
        catch (Exception ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new Exception("Neočekávaná chyba. O problému víme a pracujeme na nápravě.");
        }
    }

    public async Task SaveInvestmentAsync(InvestmentDto investment, CancellationToken cancellationToken = default)
    {
        try
        {
            await apiClient.CreateInvestmentAsync(investment, cancellationToken);
        }
        catch (ApiException ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new ApiInvestmentsResponseException("Nepodařilo se uložit investici. Zkuste to prosím později.");
        }
        catch (HttpRequestException)
        {
            throw new HttpRequestException("Nelze se připojit k internetu. Zkontrolujte prosím připojení a zkuste to znovu.");
        }
        catch (Exception ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new Exception("Neočekávaná chyba. O problému víme a pracujeme na nápravě.");
        }
    }

    public async Task UpdateInvestmentAsync(InvestmentDto investment, CancellationToken cancellationToken = default)
    {
        try
        {
            await apiClient.UpdateInvestmentAsync(investment, cancellationToken);
        }
        catch (ApiException ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new ApiInvestmentsResponseException("Nepodařilo se editovat investici. Zkuste to prosím později.");
        }
        catch (HttpRequestException)
        {
            throw new HttpRequestException("Nelze se připojit k internetu. Zkontrolujte prosím připojení a zkuste to znovu.");
        }
        catch (Exception ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new Exception("Neočekávaná chyba. O problému víme a pracujeme na nápravě.");
        }
    }

    public async Task DeleteInvestmentAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await apiClient.DeleteInvestmentAsync(id, cancellationToken);
        }
        catch (ApiException ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new ApiInvestmentsResponseException("Nepodařilo se vymazat investici. Zkuste to prosím později.");
        }
        catch (HttpRequestException)
        {
            throw new HttpRequestException("Nelze se připojit k internetu. Zkontrolujte prosím připojení a zkuste to znovu.");
        }
        catch (Exception ex)
        {
            await apiClient.SendErrorAsync(ex.ToString(), cancellationToken);
            throw new Exception("Neočekávaná chyba. O problému víme a pracujeme na nápravě.");
        }
    }
}