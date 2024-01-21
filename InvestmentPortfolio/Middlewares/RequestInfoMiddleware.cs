using InvestmentPortfolio.Services.Geolocation;

namespace InvestmentPortfolio.Middlewares;

public sealed class RequestInfoMiddleware(IGeolocationService geolocationService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string clientIp = context.Request.Headers["X-Real-IP"].ToString();
        string referer = context.Request.Headers.Referer.ToString();
        await geolocationService.GetGeolocationAsync(clientIp, referer);

        await next(context);
    }
}