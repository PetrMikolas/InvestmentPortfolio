using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Services.ExchangeRate;
using Microsoft.Extensions.Caching.Memory;

namespace InvestmentPortfolio.Services.Investment;

public class InvestmentService(IInvestmentRepository repository, IExchangeRateService exchangeRateService, IMapper mapper, IMemoryCache memoryCache) : IInvestmentService
{
    private const string INVESTMENTS_CACHE_KEY = "Investments";

    public async Task<Investments> GetAllAsync(bool hasRefreshExchangeRates, CancellationToken cancellationToken)
    {
        var exchangeRates = await GetExchangeRatesAsync(hasRefreshExchangeRates, cancellationToken);

        var investments = await memoryCache.GetOrCreateAsync(INVESTMENTS_CACHE_KEY, async entry =>
        {
            var entities = await repository.GetAllAsync(cancellationToken);
            List<Models.Investment> items = [.. entities.Select(mapper.Map<Models.Investment>)];
                   
            SetValueCzk(items, exchangeRates.Items);
            var totalSum = items.Sum(x => x.ValueCzk);
            SetPercentage(items, totalSum);

            var investments = new Investments()
            {
                Items = items,
                TotalSum = totalSum,
                ExchangeRates = exchangeRates
            };

            return investments;

        }) ?? new Investments();

        return investments;
    }

    private void SetValueCzk(List<Models.Investment> investmenst, List<Models.ExchangeRate> exchangeRates) =>
        investmenst.ForEach(item => item.ValueCzk = GetValueCzk(item.Value, item.CurrencyCode, exchangeRates));

    private long GetValueCzk(long value, string currencyCode, List<Models.ExchangeRate> exchangeRates)
    {
        if (currencyCode == "CZK")
        {
            return value;
        }

        if (exchangeRates.Count != 0)
        {
            var exchangeRate = exchangeRates.Where(x => x.Code == currencyCode).FirstOrDefault();
            return exchangeRate is not null ? (long)Math.Round(value / exchangeRate.Amount * exchangeRate.Rate) : 0;
        }
        else
        {
            return 0;
        }
    }

    private void SetPercentage(List<Models.Investment> investments, long totalSum) =>
        investments.ForEach(item => item.Percentage = $"{GetPercentage(item.ValueCzk, totalSum)} %");

    private string GetPercentage(float value, long totalSum) =>
         Math.Round(value / totalSum * 100, 2).ToString();

    private async Task<ExchangeRates> GetExchangeRatesAsync(bool isRefresh, CancellationToken cancellationToken)
    {
        string key = "ExchangeRates";

        if (isRefresh)
        {
            memoryCache.Remove(key);
        }

        var exchangeRates = await memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetAbsoluteExpiration(DateTime.Now.AddDays(1));
            memoryCache.Remove(INVESTMENTS_CACHE_KEY);
            return await exchangeRateService.GetExchangeRatesAsync(cancellationToken);

        }) ?? new ExchangeRates();

        return exchangeRates;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(id, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }

    public async Task CreateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        await repository.CreateAsync(entity, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }

    public async Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        await repository.UpdateAsync(entity, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }
}