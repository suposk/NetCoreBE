using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CommonCleanArch.Infrastructure;

public class ValidationFilterEndpoint<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
            return Results.Problem($"Validator for {typeof(T)} not found.");

        var model = context.Arguments.OfType<T>().FirstOrDefault(a => a?.GetType() == typeof(T));
        if (model is null)
            return await next(context);

        var result = await validator.ValidateAsync(model);
        if (result.IsValid is false)
            return Results.ValidationProblem(result.ToDictionary());

        return await next(context);
    }
}