using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace CommonCleanArch.Infrastructure.Infrastructure.ApiMiddleware;

public sealed class LogContextTraceLoggingMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext context)
    {
        string? traceId = Activity.Current?.TraceId.ToString();

        using (LogContext.PushProperty("TraceId", traceId))
        {
            return next.Invoke(context);
        }
    }
}
