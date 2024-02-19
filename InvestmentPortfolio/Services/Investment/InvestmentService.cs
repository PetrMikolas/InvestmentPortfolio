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
            var totalSumCzk = items.Sum(x => x.ValueCzk);
            SetPercentageShare(items, totalSumCzk);
            SetPerformanceValues(items);

            var investments = new Investments()
            {
                TotalSumCzk = totalSumCzk,
                TotalPerformanceCzk = items.Where(x => x.CurrencyCode != "CZK").Sum(x => x.PerformanceCzk),
                TotalPerformancePercentage = GetTotalPerformancePercentage(items),
                Items = items,
                ExchangeRates = exchangeRates
            };

            return investments;

        }) ?? new Investments();

        return investments;
    }

    private float GetTotalPerformancePercentage(List<Models.Investment> investmenst)
    {
        var data = investmenst.Where(x => x.CurrencyCode != "CZK");

        if (!data.Any())
        {
            return 0;
        }

        long totalDefaultValueCzk = 0;
        long totalValueCzk = 0;

        data.ToList().ForEach(investment =>
        {
            totalDefaultValueCzk += investment.DefaultValueCzk;
            totalValueCzk += investment.ValueCzk;
        });

        return (float)Math.Round(totalValueCzk / (decimal)totalDefaultValueCzk * 100 - 100, 2);
    }

    private void SetPerformanceValues(List<Models.Investment> investmenst)
    {
        foreach (var investment in investmenst)
        {
            if (investment.CurrencyCode != "CZK" && investment.ValueCzk != investment.DefaultValueCzk)
            {
                investment.PerformanceCzk = (int)(investment.ValueCzk - investment.DefaultValueCzk);
                investment.PerformancePercentage = (float)Math.Round((investment.PerformanceCzk / (decimal)investment.DefaultValueCzk) * 100, 2);
            }
        }
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
            return exchangeRate is not null ? (long)Math.Round(value / exchangeRate.Amount * (decimal)exchangeRate.Rate) : 0;
        }
        else
        {
            return 0;
        }
    }

    private void SetPercentageShare(List<Models.Investment> investments, long totalSumCzk) =>
        investments.ForEach(item => item.PercentageShare = GetPercentageShare(item.ValueCzk, totalSumCzk));

    private float GetPercentageShare(float valueCzk, long totalSum)
    {
        if (valueCzk != 0)
        {
            return (float)Math.Round(valueCzk / totalSum * 100, 2);
        }

        return 0;
    }

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
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        if (entity.CurrencyCode != "CZK")
        {
            var exchangeRates = await GetExchangeRatesAsync(true, cancellationToken);
            entity.DefaultValueCzk = GetValueCzk(entity.Value, entity.CurrencyCode, exchangeRates.Items);
        }

        await repository.CreateAsync(entity, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }

    public async Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));

        var currentInvestment = await repository.GetByIdAsync(entity.Id, cancellationToken);

        if (entity.CurrencyCode != "CZK" && entity.Value != currentInvestment.Value || entity.CurrencyCode != currentInvestment.CurrencyCode)
        {
            var exchangeRates = await GetExchangeRatesAsync(true, cancellationToken);

            currentInvestment.CreatedDate = DateTimeOffset.UtcNow;
            currentInvestment.DefaultValueCzk = GetValueCzk(entity.Value, entity.CurrencyCode, exchangeRates.Items);
        }

        currentInvestment.Name = entity.Name;
        currentInvestment.Value = entity.Value;
        currentInvestment.CurrencyCode = entity.CurrencyCode;

        await repository.UpdateAsync(currentInvestment, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }
}