using Microsoft.AspNetCore.Mvc.Filters;

namespace CommonCleanArch.Infrastructure;

public class ValidationFilterMvc : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (!filterContext.ModelState.IsValid)
        {
            filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext filterContext)
    {

    }
}
