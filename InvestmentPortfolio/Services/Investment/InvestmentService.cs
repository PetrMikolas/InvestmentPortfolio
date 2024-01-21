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
            var items = entities.Select(mapper.Map<Models.Investment>).ToList();


            SetValueCzk(items, exchangeRates);
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

    private void SetValueCzk(List<Models.Investment> investmenst, ExchangeRates exchangeRates) =>
        investmenst.ForEach(item => item.ValueCzk = GetValueCzk(item.Value, item.CurrencyCode, exchangeRates));

    private long GetValueCzk(long value, string currencyCode, ExchangeRates exchangeRates)
    {
        if (currencyCode == "CZK")
        {
            return value;
        }

        Models.ExchangeRate? exchangeRate = null;

        if (exchangeRates.Items.Count != 0)
        {
            exchangeRate = exchangeRates.Items.Where(x => x.Code == currencyCode).FirstOrDefault();
        }
        else
        {
            return 0;
        }

        if (exchangeRate is not null)
        {
            return (long)Math.Round(value / exchangeRate.Amount * exchangeRate.Rate);
        }
        else
        {
            return 0;
        }
    }

    private void SetPercentage(List<Models.Investment> investments, long totalSum) =>
        investments.ForEach(item => item.Percentage = $"{GetPercentage(item.ValueCzk, totalSum)} %");

    private float GetPercentage(float value, long totalSum) =>
        (float)Math.Round(value / totalSum * 100, 2);

    private async Task<ExchangeRates> GetExchangeRatesAsync(bool isRefresh, CancellationToken cancellationToken)
    {
        string key = "ExchangeRates";

        if (isRefresh)
        {
            memoryCache.Remove(INVESTMENTS_CACHE_KEY);
            memoryCache.Remove(key);
        }

        var exchangeRates = await memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetAbsoluteExpiration(DateTime.Now.AddDays(1));
            return await exchangeRateService.GetExchangeRatesAsync(cancellationToken);
        }) ?? new ExchangeRates();

        return exchangeRates;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(id, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }

    public async Task CreateAsync(InvestmentEntity entity, CancellationToken cancellationToken)
    {
        await repository.CreateAsync(entity, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }

    public async Task UpdateAsync(InvestmentEntity entity, CancellationToken cancellationToken)
    {
        await repository.UpdateAsync(entity, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }
}