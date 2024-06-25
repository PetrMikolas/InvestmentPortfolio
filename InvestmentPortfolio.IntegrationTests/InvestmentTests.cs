using FluentAssertions;
using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace InvestmentPortfolio.IntegrationTests;

internal class InvestmentTests
{
    private HttpClient _httpClient;
    private InvestmentEntity _firstInvestment;

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        var factory = new CustomWebApplicationFactory();
        _httpClient = factory.CreateDefaultClient();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();
        
        Utilities.InitializeDbForTesting(dbContext);
        _firstInvestment = dbContext.Investments.First();
    }

    [Test]
    public async Task GetInvestments_Success()
    {
        // Arrange 
        new WebHostBuilder()
            .UseKestrel()
            .Configure(app =>
            {
                app.Run(async context =>
                {
                    if (context.Request.Method == HttpMethods.Get && context.Request.Path == "/dummyapicnb")
                    {
                        await context.Response.WriteAsync("07.12.2023 #236\n" +
                                                          "země|měna|množství|kód|kurz\n" +
                                                          "EMU|euro|1|EUR|24,50\n" +
                                                          "USA|dolar|1|USD|22,50");
                    }
                });
            })
            .Start();              

        // Act
        var response = await _httpClient.GetAsync($"investments?RefreshExchangeRates=false");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNull();

        var responseData = JsonSerializer.Deserialize<Investments>(responseContent, Constants.DefaultJsonSerializerOptions)!;
        responseData.Should().NotBeNull();
        responseData.Items.Should().NotBeNullOrEmpty().And.HaveCount(3);
        responseData.Items.Should().Subject.First().Id.Should().Be(_firstInvestment.Id);
        responseData.Items.Should().Subject.First().Name.Should().Be(_firstInvestment.Name);
        responseData.Items.Should().Subject.First().Value.Should().Be(_firstInvestment.Value);
        responseData.Items.Should().Subject.First().CurrencyCode.Should().Be(_firstInvestment.CurrencyCode);
        responseData.TotalSumCzk.Should().Be(57000);
        responseData.ExchangeRates.Should().NotBeNull();
        responseData.ExchangeRates.Items.Should().NotBeNullOrEmpty().And.HaveCount(2);
        responseData.ExchangeRates.Date.Should().Be("07.12.2023");
        responseData.ExchangeRates.Items.Should().Subject.First().Country.Should().Be("EMU");
        responseData.ExchangeRates.Items.Should().Subject.First().Currency.Should().Be("euro");
        responseData.ExchangeRates.Items.Should().Subject.First().Amount.Should().Be(1);
        responseData.ExchangeRates.Items.Should().Subject.First().Code.Should().Be("EUR");
        responseData.ExchangeRates.Items.Should().Subject.First().Rate.Should().Be(24.50f);
    }

    [Test]
    public async Task DeleteInvestment_Success()
    {

        // Arrange + Act
        var response = await _httpClient.DeleteAsync("investments/1");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteInvestment_BadRequest()
    {
        // Arrange + Act
        var response = await _httpClient.DeleteAsync("investments/0");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteInvestment_NotFound()
    {
        // Arrange + Act
        var response = await _httpClient.DeleteAsync("investments/4");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateInvestment_Success()
    {
        // Arrange         
        var investment = new Investment() { Id = 1, Name = "Update Investment", Value = 5000, CurrencyCode = "PLN" };

        // Act
        var response = await _httpClient.PutAsJsonAsync("investments", investment);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task UpdateInvestment_NotFound()
    {
        // Arrange      
        var investment = new Investment() { Id = 4, Name = "Update Investment", Value = 5000, CurrencyCode = "PLN" };

        // Act
        var response = await _httpClient.PutAsJsonAsync("investments", investment);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task CreateInvestment_Success()
    {
        // Arrange       
        var investment = new Investment() { Name = "Create Investment", Value = 1000, CurrencyCode = "GBP" };

        // Act
        var response = await _httpClient.PostAsJsonAsync("investments", investment);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);    
    }
}