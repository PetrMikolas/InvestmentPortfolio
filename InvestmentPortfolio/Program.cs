using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Radzen;
using InvestmentPortfolio.Api.ErrorsClient;
using InvestmentPortfolio.Api.Geolocation;
using InvestmentPortfolio.Api.Investments;
using InvestmentPortfolio.Client.Services.Api;
using InvestmentPortfolio.Client.Services.Export;
using InvestmentPortfolio.Components;
using InvestmentPortfolio.Database.Geolocation;
using InvestmentPortfolio.Database.Investment;
using InvestmentPortfolio.Mappers;
using InvestmentPortfolio.Middlewares;
using InvestmentPortfolio.Sentry;
using InvestmentPortfolio.Services.Email;
using InvestmentPortfolio.Services.ExchangeRate;
using InvestmentPortfolio.Services.Geolocation;
using InvestmentPortfolio.Services.Investment;
using static InvestmentPortfolio.Services.Email.EmailService;

var builder = WebApplication.CreateBuilder(args);

// Add Sentry services to the WebHostBuilder.
builder.WebHost.AddSentry(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<RequestInfoMiddleware>();
builder.Services.AddMemoryCache();

// Register and configure AutoMapper with custom profile.
builder.Services.AddAutoMapper(config =>
    config.AddProfile<AutoMapperProfile>(), typeof(Program).Assembly);

// Register MediatR services for handling commands and queries.
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Configure options for email service.
builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.Key))
    .ValidateDataAnnotations();

// Create and configure email service instance.
var loggerEmailService = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<EmailService>();
var optionsEmailService = Options.Create(new EmailOptions());
var emailService = new EmailService(optionsEmailService, loggerEmailService);

// Add Data Protection with persistent keys
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/Keys"))
    .SetApplicationName("investment-portfolio");

// Register API explorer and Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocument();

// Register database-related services.
builder.Services.AddInvestmentDatabase(builder.Configuration, emailService);
builder.Services.AddGeolocationDatabase(builder.Configuration, emailService);

// Register application services and configure HttpClient for GeolocationService.
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddTransient<IInvestmentService, InvestmentService>();
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>();

// Register client services and configure HttpClient for ApiClient.
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(config =>
    config.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!));

var app = builder.Build();

// Apply RequestInfoMiddleware only for requests to the root path ("/").
app.UseWhen(
    context => context.Request.Path.Equals("/"),
    appBuilder => appBuilder.UseMiddleware<RequestInfoMiddleware>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable debugging and Swagger UI in development.
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Handle exceptions and enforce HTTPS in production.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

// Not used in Docker – HTTPS is handled by the proxy (avoids warning or redirect loop)
//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Configure Razor Components.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(InvestmentPortfolio.Client._Imports).Assembly);

// Apply database migrations
app.UseInvestmentDatabase(emailService);
app.UseGeolocationDatabase(emailService);

// Map application endpoints.
app.MapInvestmentEndpoints();
app.MapGeolocationEndpoints();
app.MapClientErrorEndpoints();

app.Run();