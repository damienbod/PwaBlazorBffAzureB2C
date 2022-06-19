using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlazorHosted.Server;

public class CsrfProtectionCorsPreflightAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var header = context.HttpContext.Request.Headers.Any(p => p.Key.ToLower() == "x-force-cors-preflight");
        if (!header)
        {
            // "X-FORCE-CORS-PREFLIGHT header is missing"
            context.Result = new UnauthorizedObjectResult("X-FORCE-CORS-PREFLIGHT header is missing");
            return;
        }
    }
}
