using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CommonBE.Infrastructure.ApiMiddleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    //private readonly ITelemetryService _telemetryService;
    private readonly ApiExceptionOptions _options;

    public IServiceProvider Service { get; }


    public ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next,
        IServiceProvider service,
        ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        Service = service;
        _logger = logger;
        _options = options;
        using var scope = Service.CreateScope();
        //_telemetryService = scope.ServiceProvider.GetService<ITelemetryService>(); //must do this, different scope
    }

    public async Task Invoke(HttpContext context /* other dependencies */)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            HandleExceptionAsync(context, ex);
            //throw; // Just let exception be thrown so caller can process it
        }
    }

    const string ERRORText = "Error occurred in the API. Please use the ErrorId and contact our support team if the problem persists.";

    private void HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new ErrorServerModel
        {
            ApiErrorId = Guid.NewGuid().ToString(),
            //ApiStatus = (int)HttpStatusCode.InternalServerError,
        };

        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        switch (exception)
        {
            case BadRequestException badRequestException:
                httpStatusCode = HttpStatusCode.BadRequest;
                error.ApiTitle = $"{badRequestException.Message} {ERRORText}";
                break;
            case NotFoundException notFoundException:
                httpStatusCode = HttpStatusCode.NotFound;
                error.ApiTitle = $"{notFoundException.Message} {ERRORText}";
                break;
            case UnauthorizedException unauthorizedException:
                httpStatusCode = HttpStatusCode.Unauthorized;
                error.ApiTitle = $"{unauthorizedException.Message} {ERRORText}";
                break;
            case ForbiddenException forbiddenException:
                httpStatusCode = HttpStatusCode.Forbidden;
                error.ApiTitle = $"{forbiddenException.Message} {ERRORText}";
                break;
            case ConflictException conflictException:
                httpStatusCode = HttpStatusCode.Conflict;
                error.ApiTitle = $"{conflictException.Message} {ERRORText}";
                break;
            case Exception ex:
                httpStatusCode = HttpStatusCode.InternalServerError;
                error.ApiTitle = $"{ex.Message} {ERRORText}";
                break;
        }
        error.ApiStatus = (int)httpStatusCode;

        _options.AddResponseDetails?.Invoke(context, exception, error);

        //_telemetryService?.TrackException(exception);
        var innerExMessage = GetInnermostExceptionMessage(exception);
        var level = _options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;
        _logger.Log(level, exception, "ApiExceptionMiddleware!!! " + innerExMessage + " -- {ErrorId}.", error.ApiErrorId);

        //var result = JsonConvert.SerializeObject(error);
        var result = JsonSerializer.Serialize(error);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        context.Response.WriteAsync(result);
    }

    private string GetInnermostExceptionMessage(Exception exception)
    {
        if (exception.InnerException != null)
            return GetInnermostExceptionMessage(exception.InnerException);

        return exception.Message;
    }
}
