using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Services.ExchangeRate;
using Microsoft.Extensions.Caching.Memory;

namespace InvestmentPortfolio.Services.Investment;

/// <summary>
/// Service responsible for managing investments.
/// </summary>
/// <param name="repository">The repository for managing investments.</param>
/// <param name="exchangeRateService">The service for exchange rates.</param>
/// <param name="mapper">The AutoMapper instance for mapping entities to models.</param>
/// <param name="memoryCache">The memory cache for caching investments and exchange rates.</param>
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
                TotalPerformancePercentage = CalculateTotalPerformancePercentage(items),
                Items = items,
                ExchangeRates = exchangeRates
            };

            return investments;

        }) ?? new Investments();

        return investments;
    }

    /// <summary>
    /// Calculates the total performance percentage of investments.
    /// </summary>
    /// <param name="investments">The list of investments.</param>
    /// <returns>Returns the total performance percentage.</returns>
    private float CalculateTotalPerformancePercentage(List<Models.Investment> investments)
    {
        var data = investments.Where(x => x.CurrencyCode != "CZK");

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

    /// <summary>
    /// Sets the performance values for each investment.
    /// </summary>
    /// <param name="investments">The list of investments.</param>
    private void SetPerformanceValues(List<Models.Investment> investments)
    {
        foreach (var investment in investments)
        {
            if (investment.CurrencyCode != "CZK" && investment.ValueCzk != investment.DefaultValueCzk)
            {
                investment.PerformanceCzk = (int)(investment.ValueCzk - investment.DefaultValueCzk);
                investment.PerformancePercentage = (float)Math.Round(investment.PerformanceCzk / (decimal)investment.DefaultValueCzk * 100, 2);
            }
        }
    }

    /// <summary>
    /// Sets the value in CZK for each investment.
    /// </summary>
    /// <param name="investments">The list of investments.</param>
    /// <param name="exchangeRates">The list of exchange rates.</param>
    private void SetValueCzk(List<Models.Investment> investments, List<Models.ExchangeRate> exchangeRates) =>
        investments.ForEach(item => item.ValueCzk = CalculateValueCzk(item.Value, item.CurrencyCode, exchangeRates));

    /// <summary>
    /// Calculates the value of an investment in CZK (Czech Koruna) based on its value in the original currency and current exchange rates.
    /// </summary>
    /// <param name="value">The value of the investment in the original currency.</param>
    /// <param name="currencyCode">The currency code of the investment.</param>
    /// <param name="exchangeRates">The list of exchange rates.</param>
    /// <returns>Returns the value of the investment in CZK.</returns>
    private long CalculateValueCzk(long value, string currencyCode, List<Models.ExchangeRate> exchangeRates)
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

    /// <summary>
    /// Sets the percentage share for each investment.
    /// </summary>
    /// <param name="investments">The list of investments.</param>
    /// <param name="totalSumCzk">The total sum in CZK.</param>
    private void SetPercentageShare(List<Models.Investment> investments, long totalSumCzk) =>
        investments.ForEach(item => item.PercentageShare = CalculatePercentageShare(item.ValueCzk, totalSumCzk));

    /// <summary>
    /// Calculates the percentage share of an investment value in relation to the total sum.
    /// </summary>
    /// <param name="valueCzk">The value of the investment in CZK.</param>
    /// <param name="totalSum">The total sum of investments in CZK.</param>
    /// <returns>Returns the percentage share of the investment value in relation to the total sum.</returns>
    private float CalculatePercentageShare(float valueCzk, long totalSum)
    {
        if (valueCzk != 0)
        {
            return (float)Math.Round(valueCzk / totalSum * 100, 2);
        }

        return 0;
    }

    /// <summary>
    /// Gets exchange rates asynchronously.
    /// </summary>
    /// <param name="isRefresh">Indicates whether to refresh exchange rates.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the exchange rates.</returns>
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
        ArgumentNullException.ThrowIfNull(entity);        

        if (entity.CurrencyCode != "CZK")
        {
            var exchangeRates = await GetExchangeRatesAsync(true, cancellationToken);
            entity.DefaultValueCzk = CalculateValueCzk(entity.Value, entity.CurrencyCode, exchangeRates.Items);
        }

        await repository.CreateAsync(entity, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }

    public async Task UpdateAsync(InvestmentEntity? entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var currentInvestment = await repository.GetByIdAsync(entity.Id, cancellationToken);

        if (entity.CurrencyCode != "CZK" && entity.Value != currentInvestment.Value || entity.CurrencyCode != currentInvestment.CurrencyCode)
        {
            var exchangeRates = await GetExchangeRatesAsync(true, cancellationToken);

            currentInvestment.CreatedDate = DateTimeOffset.UtcNow;
            currentInvestment.DefaultValueCzk = CalculateValueCzk(entity.Value, entity.CurrencyCode, exchangeRates.Items);
        }

        currentInvestment.Name = entity.Name;
        currentInvestment.Value = entity.Value;
        currentInvestment.CurrencyCode = entity.CurrencyCode;

        await repository.UpdateAsync(currentInvestment, cancellationToken);
        memoryCache.Remove(INVESTMENTS_CACHE_KEY);
    }
}