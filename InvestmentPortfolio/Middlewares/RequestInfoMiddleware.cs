﻿using InvestmentPortfolio.Services.Geolocation;

namespace InvestmentPortfolio.Middlewares;

/// <summary>
/// Middleware for capturing request information.
/// </summary>
/// <param name="geolocationService">The geolocation service.</param>
public sealed class RequestInfoMiddleware(IGeolocationService geolocationService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {        
        var cancellationToken = context.RequestAborted;
        string clientIp = context.Request.Headers["X-Real-IP"].ToString();
        string referer = context.Request.Headers.Referer.ToString();
        await geolocationService.GetGeolocationAsync(clientIp, referer, cancellationToken);

        await next(context);
    }
}