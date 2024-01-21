using InvestmentPortfolio.Client.Models;
using InvestmentPortfolio.Client.Services.Api;
using FluentAssertions;
using InvestmentPortfolio.Client.Exceptions;

namespace InvestmentPortfolio.Client.UnitTests;

internal class ApiServiceTests
{
    private ApiService _apiService;

    [SetUp]
    public void Setup()
    {
        _apiService = new ApiService(new HttpClient());
    }

    [Test]
    public void SetAllValues_Success()
    {
        // Arrange 
        var investments = new List<Investment>
        {
            new()
            {
                Id = 1,
                Name = "Investice CZK",
                Value = 22500,
                CurrencyCode = "CZK"
            },
            new()
            {
                Id = 2,
                Name = "Investice USD",
                Value = 1000,
                CurrencyCode = "USD"
            },
            new()
            {
                Id = 3,
                Name = "Investice HUF",
                Value = 300000,
                CurrencyCode = "HUF"
            }
        };

        var exchangeRates = new ExchangeRates()
        {
            Date = DateTime.Now.Date.ToShortDateString(),
            Items = new List<ExchangeRate>
            {
                new()
                {
                    Country = "USA", Code = "USD", Amount = 1, Currency = "dolar", Rate = 22.50f
                },
                new()
                {
                    Country = "Maďarsko", Code = "HUF", Amount = 100, Currency = "forint", Rate = 7.50f
                }
            }
        };

        _apiService.Investments = investments;
        _apiService.ExchangeRates = exchangeRates;
        string percentage = "33,33 %";

        // Act
        _apiService.SetAllValues();

        // Assert
        _apiService.Investments.Should().NotBeNullOrEmpty().And.HaveCount(3);
        _apiService.Investments.Should().Subject.ElementAt(0).ValueCzk.Should().Be(22500);
        _apiService.Investments.Should().Subject.ElementAt(0).Percentage.Should().Be(percentage);
        _apiService.Investments.Should().Subject.ElementAt(0).NamePercentage.Should().Be($"{investments[0].Name} ({percentage})");
        _apiService.Investments.Should().Subject.ElementAt(1).ValueCzk.Should().Be(22500);
        _apiService.Investments.Should().Subject.ElementAt(1).Percentage.Should().Be(percentage);
        _apiService.Investments.Should().Subject.ElementAt(1).NamePercentage.Should().Be($"{investments[1].Name} ({percentage})");
        _apiService.Investments.Should().Subject.ElementAt(2).ValueCzk.Should().Be(22500);
        _apiService.Investments.Should().Subject.ElementAt(2).Percentage.Should().Be(percentage);
        _apiService.Investments.Should().Subject.ElementAt(2).NamePercentage.Should().Be($"{investments[2].Name} ({percentage})");
        _apiService.InvestmentsTotalSum.Should().Be(67500);
    }

    [Test]
    public void SetAllValues_ExchangeRateNotFound_Throws_ExchangeRateNotFoundException()
    {
        // Arrange 
        var investments = new List<Investment>
        {
            new()
            {
                Id = 3,
                Name = "Investice HUF",
                Value = 300000,
                CurrencyCode = "HUF"
            }
        };

        var exchangeRates = new ExchangeRates()
        {
            Date = DateTime.Now.Date.ToShortDateString(),
            Items = new List<ExchangeRate>
            {
                new()
                {
                    Country = "USA", Code = "USD", Amount = 1, Currency = "dolar", Rate = 22.50f
                }
            }
        };

        _apiService.Investments = investments;
        _apiService.ExchangeRates = exchangeRates;

        // Act + Assert
        Assert.Throws<ExchangeRateNotFoundException>(_apiService.SetAllValues);
    }
}