using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace CommonBE.Infrastructure.ApiMiddleware;

public static class ApiExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder)
    {
        var options = new ApiExceptionOptions();
        return builder.UseMiddleware<ApiExceptionMiddleware>(options);
    }

    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder,
        Action<ApiExceptionOptions> configureOptions = null)
    {
        var options = new ApiExceptionOptions();
        configureOptions(options);

        return builder.UseMiddleware<ApiExceptionMiddleware>(options);
    }

    public static LogLevel DetermineLogLevel(Exception ex)
    {
        if (ex.Message.StartsWith("cannot open database", StringComparison.InvariantCultureIgnoreCase) ||
            ex.Message.StartsWith("a network-related", StringComparison.InvariantCultureIgnoreCase))
        {
            return LogLevel.Critical;
        }
        return LogLevel.Error;
    }

    public static void UpdateApiErrorResponse(HttpContext context, Exception ex, ErrorServerModel error)
    {
        if (ex.GetType().Name == nameof(DbException) || ex.GetType().Name == nameof(SqlException))
        {
            error.ApiDetail = "Exception was a database exception!";
        }
        //error.Links = "https://gethelpformyerror.com/";
    }
}
