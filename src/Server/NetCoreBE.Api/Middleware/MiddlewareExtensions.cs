using CommonCleanArch.Infrastructure.Infrastructure.ApiMiddleware;

namespace NetCoreBE.Api.Middleware;

internal static class MiddlewareExtensions
{
    internal static IApplicationBuilder UseLogContext(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogContextTraceLoggingMiddleware>();

        return app;
    }
}
