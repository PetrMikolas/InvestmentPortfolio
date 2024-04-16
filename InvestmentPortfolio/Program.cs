using InvestmentPortfolio.Api.ErrorsClient;
using InvestmentPortfolio.Api.Geolocation;
using InvestmentPortfolio.Api.Investments;
using InvestmentPortfolio.Client.Services.Api;
using InvestmentPortfolio.Client.Services.Export;
using InvestmentPortfolio.Components;
using InvestmentPortfolio.Database;
using InvestmentPortfolio.Database.Geolocation;
using InvestmentPortfolio.Mappers;
using InvestmentPortfolio.Middlewares;
using InvestmentPortfolio.Services.Email;
using InvestmentPortfolio.Services.ExchangeRate;
using InvestmentPortfolio.Services.Geolocation;
using InvestmentPortfolio.Services.Investment;
using Microsoft.Extensions.Options;
using Radzen;
using static InvestmentPortfolio.Services.Email.EmailService;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration["SentryDsn"]!;
    o.Debug = false;
    o.TracesSampleRate = 1.0;
});

builder.Services.AddScoped<RequestInfoMiddleware>();
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.Key))
    .ValidateDataAnnotations();

var loggerEmailService = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<EmailService>();
var optionsEmailService = Options.Create(new EmailOptions());
var emailService = new EmailService(optionsEmailService, loggerEmailService);

// Database
builder.Services.AddDatabaseInvestment(builder.Configuration, emailService);
builder.Services.AddDatabaseGeolocation(builder.Configuration, emailService);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocument();

builder.Services.AddAutoMapper(config =>
    config.AddProfile<AutoMapperProfile>(), typeof(Program).Assembly);

builder.Services.AddTransient<IInvestmentService, InvestmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>();

// Client service
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(config =>
    config.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!));

var app = builder.Build();

app.UseWhen(
    context => context.Request.Path.Equals("/"),
    appBuilder => appBuilder.UseMiddleware<RequestInfoMiddleware>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(InvestmentPortfolio.Client._Imports).Assembly);

// Database
app.UseDatabaseInvestment(emailService);
app.UseDatabaseGeolocation(emailService);

// minimal API
app.MapEndpointsInvestments();
app.MapEndpointsGeolocation();
app.MapEndpointsErrorsClient();

app.Run();