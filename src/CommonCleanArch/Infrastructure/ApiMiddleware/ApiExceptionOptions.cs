using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommonCleanArch.Infrastructure.ApiMiddleware;

public class ApiExceptionOptions
{
    public Action<HttpContext, Exception, ErrorServerModel> AddResponseDetails { get; set; }
    public Func<Exception, LogLevel> DetermineLogLevel { get; set; }
}
