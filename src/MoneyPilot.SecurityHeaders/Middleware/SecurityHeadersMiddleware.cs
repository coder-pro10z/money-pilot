using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MoneyPilot.SecurityHeaders.Options;

namespace MoneyPilot.SecurityHeaders.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(RequestDelegate next, IOptions<SecurityHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add headers that are safe to send on every response
        context.Response.Headers.Append("X-Frame-Options", _options.XFrameOptions);
        context.Response.Headers.Append("X-Content-Type-Options", _options.XContentTypeOptions);
        context.Response.Headers.Append("Referrer-Policy", _options.ReferrerPolicy);

        // HSTS – only add if the request is over HTTPS
        if (context.Request.IsHttps)
        {
            var hstsValue = $"max-age={_options.HstsMaxAge}";
            if (_options.HstsIncludeSubDomains)
                hstsValue += "; includeSubDomains";
            if (_options.HstsPreload)
                hstsValue += "; preload";
            context.Response.Headers.Append("Strict-Transport-Security", hstsValue);
        }

        // Content Security Policy
        context.Response.Headers.Append("Content-Security-Policy", _options.ContentSecurityPolicy);

        await _next(context);
    }
}