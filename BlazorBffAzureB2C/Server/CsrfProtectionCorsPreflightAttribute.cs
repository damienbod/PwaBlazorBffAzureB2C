using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace BlazorHosted.Server
{
    public class CsrfProtectionCorsPreflightAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var header = context.HttpContext.Request.Headers.Any(p => p.Key.ToLower() == "x-force-cors-preflight");
            if (!header)
            {
                context.Result = new BadRequestObjectResult("X-FORCE-CORS-PREFLIGHT header is missing");
                return;
            }
        }
    }
}
