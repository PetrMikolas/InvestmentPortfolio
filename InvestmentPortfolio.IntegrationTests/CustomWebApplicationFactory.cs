using InvestmentPortfolio.Database.Investment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InvestmentPortfolio.IntegrationTests;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<InvestmentDbContext>>();

            services.AddDbContext<InvestmentDbContext>(options =>
            {
                options.UseInMemoryDatabase("InvestmentTest");
            });
        });
    }

    internal HttpClient CreateDefaultClient()
    {
        var client = WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTests");
            builder.ConfigureAppConfiguration(config => { });
            builder.ConfigureTestServices(services => { });

        }).CreateClient();

        return client;
    }
}