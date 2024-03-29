using InvestmentPortfolio.Client.Services.Api;
using InvestmentPortfolio.Client.Services.Export;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => 
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddRadzenComponents();

await builder.Build().RunAsync();