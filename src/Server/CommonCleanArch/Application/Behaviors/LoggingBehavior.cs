using Microsoft.Extensions.Logging;
using SharedCommon;
using Serilog.Context;

namespace CommonCleanArch.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : ResultCom
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;

        try
        {
            _logger.LogInformation("Executing request {RequestName}", requestName);
            TResponse result = await next();

            if (result.IsSuccess)            
                _logger.LogInformation("Request {RequestName} processed successfully", requestName);            
            else
            {
                using (LogContext.PushProperty("Error", result.ErrorMessage, true))
                {
                    _logger.LogError("Request {RequestName} processed with error", requestName);
                }
            }
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Request {RequestName} processing failed", requestName);
            throw;
        }
    }
}
