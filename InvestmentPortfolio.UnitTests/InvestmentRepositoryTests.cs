using FluentAssertions;
using InvestmentPortfolio.Exceptions;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Repositories.Investment;

namespace InvestmentPortfolio.UnitTests;

internal class InvestmentRepositoryTests
{
    private InvestmentRepository _investmentRepository;

    [SetUp]
    public void Setup()
    {
        var dbContext = new InMemoryDbContextWrapper().DbContext;
        _investmentRepository = new InvestmentRepository(dbContext);
    }

    [Test]
    public async Task GetAll_Success()
    {
        // Act
        var result = await _investmentRepository.GetAllAsync(default);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Count().Should().BeGreaterThanOrEqualTo(3);
    }

    [Test]
    public async Task GetByIdAsync_Success()
    {
        // Arrange
        var id = 1;

        // Act
        var result = await _investmentRepository.GetByIdAsync(id, default);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void GetById_NotFound_Throws_EntityNotFoundException()
    {
        // Arrange
        var nonExistingId = 99999;

        // Act + Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _investmentRepository.GetByIdAsync(nonExistingId, default));
    }

    [Test]
    public async Task Update_Success()
    {
        // Arrange
        var investment = new InvestmentEntity()
        {
            Id = 1,
            Name = "Investment",
            Value = 1000,
            CurrencyCode = "EUR"
        };

        // Act
        await _investmentRepository.UpdateAsync(investment, default);

        // Assert
        Assert.Pass();
    }

    [Test]
    public void Update_NotFound_Throws_EntityNotFoundException()
    {
        // Arrange
        var investment = new InvestmentEntity()
        {
            Id = 9999,
            Name = "Investment",
            Value = 1000,
            CurrencyCode = "EUR"
        };

        // Act + Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _investmentRepository.UpdateAsync(investment, default));
    }

    [Test]
    public async Task Create_SuccessAsync()
    {
        // Arrange
        var investment = new InvestmentEntity()
        {
            Name = "Investment",
            Value = 1000,
            CurrencyCode = "EUR"
        };

        // Act
        await _investmentRepository.CreateAsync(investment, default);

        // Assert
        Assert.Pass();
    }

    [Test]
    public async Task Delete_Success()
    {
        // Arrange
        var id = 1;

        // Act
        await _investmentRepository.DeleteAsync(id, default);

        // Assert
        Assert.Pass();
    }

    [Test]
    public void Delete_NotFound_Throws_EntityNotFoundException()
    {
        // Arrange
        var nonExistingId = 99999;

        // Act + Assert
        Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _investmentRepository.DeleteAsync(nonExistingId, default));
    }
}