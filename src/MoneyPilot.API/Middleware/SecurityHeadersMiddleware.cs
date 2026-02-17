using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MoneyPilot.API.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers to every response
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // HSTS – only add if the request is over HTTPS
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }

            // Content Security Policy (adjust according to your needs)
            string csp = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self';";
            context.Response.Headers.Append("Content-Security-Policy", csp);

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}   