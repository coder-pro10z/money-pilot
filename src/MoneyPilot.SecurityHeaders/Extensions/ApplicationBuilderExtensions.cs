using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MoneyPilot.SecurityHeaders.Middleware;
using MoneyPilot.SecurityHeaders.Options;

namespace MoneyPilot.SecurityHeaders.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMoneyPilotSecurityHeaders(this IApplicationBuilder app)
    {
        // Ensure the options are registered (they can be configured via DI)
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }

    // Optional: Add a method to configure options easily
    public static IServiceCollection AddMoneyPilotSecurityHeaders(
        this IServiceCollection services,
        Action<SecurityHeadersOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
}