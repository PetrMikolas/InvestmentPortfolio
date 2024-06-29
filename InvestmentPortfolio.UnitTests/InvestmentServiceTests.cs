using AutoMapper;
using FluentAssertions;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Services.ExchangeRate;
using InvestmentPortfolio.Services.Investment;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Snapshooter.NUnit;

namespace InvestmentPortfolio.UnitTests;

internal class InvestmentServiceTests
{
    private Mock<IInvestmentRepository> _mockInvestmentRepository;
    private Mock<IExchangeRateService> _mockExchangeRateService;
    private InvestmentService _investmentService;

    [SetUp]
    public void Setup()
    {
        _mockInvestmentRepository = new Mock<IInvestmentRepository>();
        _mockExchangeRateService = new Mock<IExchangeRateService>();

        var mapperConfig = new MapperConfiguration(config => config.AddMaps(["InvestmentPortfolio"]));
        IMapper mapper = mapperConfig.CreateMapper();

        var mockMemoryCache = new Mock<IMemoryCache>();
        var cachEntry = Mock.Of<ICacheEntry>();
        mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<string>())).Returns(cachEntry);

        _investmentService = new InvestmentService(_mockInvestmentRepository.Object, _mockExchangeRateService.Object, mapper, mockMemoryCache.Object);
    }

    [Test]
    public async Task GetAll_Success()
    {
        // Arrange 
        var entities = new List<InvestmentEntity>
        {
            new() { Id = 1, Name = "Investice CZK", Value = 22500, CurrencyCode = "CZK" },
            new() { Id = 2, Name = "Investice USD", Value = 1000, CurrencyCode = "USD", DefaultValueCzk = 21375 },
            new() { Id = 3, Name = "Investice HUF", Value = 300000, CurrencyCode = "HUF", DefaultValueCzk = 21375  }
        };

        var exchangeRates = new ExchangeRates()
        {
            Date = "17.01.2024",
            Items = new List<ExchangeRate>
            {
                new() { Country = "USA", Code = "USD", Amount = 1, Currency = "dolar", Rate = 22.50f },
                new() { Country = "Maďarsko", Code = "HUF", Amount = 100, Currency = "forint", Rate = 7.50f }
            }
        };

        _mockInvestmentRepository.Setup(x => x.GetAllAsync(default)).ReturnsAsync(entities);
        _mockExchangeRateService.Setup(x => x.GetAsync(default)).ReturnsAsync(exchangeRates);

        // Act
        var response = await _investmentService.GetAllAsync(false, default);

        // Assert
        _mockInvestmentRepository.Verify(x => x.GetAllAsync(default), Times.Once());
        _mockExchangeRateService.Verify(x => x.GetAsync(default), Times.Once());

        response.Should().NotBeNull();
        response.Items.Should().NotBeNullOrEmpty().And.HaveCount(3);
        response.TotalSumCzk.Should().NotBe(0);
        response.TotalPerformanceCzk.Should().Be(2250);
        response.TotalPerformancePercentage.Should().Be(5.26f);
        response.ExchangeRates.Should().NotBeNull();
        response.ExchangeRates.Items.Should().NotBeNullOrEmpty().And.HaveCount(2);
        response.ExchangeRates.Date.Should().NotBeNullOrEmpty();
        Snapshot.Match(response);
    }

    [Test]
    public async Task GetAll_ExchangeRatesIsEmpty_Success()
    {
        // Arrange 
        var entities = new List<InvestmentEntity>
        {
            new() { Id = 1, Name = "Investice CZK", Value = 22500, CurrencyCode = "CZK" },
            new() { Id = 2, Name = "Investice USD", Value = 1000, CurrencyCode = "USD", DefaultValueCzk = 21375 },
            new() { Id = 3, Name = "Investice HUF", Value = 300000, CurrencyCode = "HUF", DefaultValueCzk = 21375  }
        };

        var exchangeRatesEmpty = new ExchangeRates();

        _mockInvestmentRepository.Setup(x => x.GetAllAsync(default)).ReturnsAsync(entities);
        _mockExchangeRateService.Setup(x => x.GetAsync(default)).ReturnsAsync(exchangeRatesEmpty);

        // Act
        var response = await _investmentService.GetAllAsync(false, default);

        // Assert
        _mockInvestmentRepository.Verify(x => x.GetAllAsync(default), Times.Once());
        _mockExchangeRateService.Verify(x => x.GetAsync(default), Times.Once());

        response.Should().NotBeNull();
        response.Items.Should().NotBeNullOrEmpty().And.HaveCount(3);
        response.TotalSumCzk.Should().Be(22500);
        response.TotalPerformanceCzk.Should().Be(-42750);
        response.TotalPerformancePercentage.Should().Be(-100);
        response.ExchangeRates.Should().NotBeNull();
        response.ExchangeRates.Items.Should().NotBeNull().And.HaveCount(0);
        response.ExchangeRates.Date.Should().NotBeNull();
        Snapshot.Match(response);
    }

    [Test]
    public async Task GetAll_InvestmentEntityIsEmpty_Success()
    {
        // Arrange 
        var entitiesEmpty = new List<InvestmentEntity>();

        var exchangeRates = new ExchangeRates()
        {
            Date = "17.01.2024",
            Items = new List<ExchangeRate>
            {
                new() { Country = "USA", Code = "USD", Amount = 1, Currency = "dolar", Rate = 22.50f },
                new() { Country = "Maďarsko", Code = "HUF", Amount = 100, Currency = "forint", Rate = 7.50f }
            }
        };

        _mockInvestmentRepository.Setup(x => x.GetAllAsync(default)).ReturnsAsync(entitiesEmpty);
        _mockExchangeRateService.Setup(x => x.GetAsync(default)).ReturnsAsync(exchangeRates);

        // Act
        var response = await _investmentService.GetAllAsync(false, default);

        // Assert
        _mockInvestmentRepository.Verify(x => x.GetAllAsync(default), Times.Once());
        _mockExchangeRateService.Verify(x => x.GetAsync(default), Times.Once());

        response.Should().NotBeNull();
        response.Items.Should().NotBeNull().And.HaveCount(0);
        response.TotalSumCzk.Should().Be(0);
        response.ExchangeRates.Should().NotBeNull();
        response.ExchangeRates.Items.Should().NotBeNullOrEmpty().And.HaveCount(2);
        response.ExchangeRates.Date.Should().NotBeNullOrEmpty();
        Snapshot.Match(response);
    }

    [Test]
    public async Task GetAll_InvestmentEntityAndExchangeRatesIsEmpty_Success()
    {
        // Arrange 
        var entitiesEmpty = new List<InvestmentEntity>();
        var exchangeRatesEmpty = new ExchangeRates();

        _mockInvestmentRepository.Setup(x => x.GetAllAsync(default)).ReturnsAsync(entitiesEmpty);
        _mockExchangeRateService.Setup(x => x.GetAsync(default)).ReturnsAsync(exchangeRatesEmpty);

        // Act
        var response = await _investmentService.GetAllAsync(false, default);

        // Assert
        _mockInvestmentRepository.Verify(x => x.GetAllAsync(default), Times.Once());
        _mockExchangeRateService.Verify(x => x.GetAsync(default), Times.Once());

        response.Should().NotBeNull();
        response.Items.Should().NotBeNull().And.HaveCount(0);
        response.TotalSumCzk.Should().Be(0);
        response.ExchangeRates.Should().NotBeNull();
        response.ExchangeRates.Items.Should().NotBeNull().And.HaveCount(0);
        response.ExchangeRates.Date.Should().NotBeNull();
        Snapshot.Match(response);
    }
}