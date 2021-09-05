using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BlazorHosted.Server.Controllers
{
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [AuthorizeForScopes(Scopes = new string[] { "https://b2cdamienbod.onmicrosoft.com/e207bcb0-1a36-4d37-a9cd-b917bb1366b1/access_as_user" })]
    [ApiController]
    [Route("api/[controller]")]
    public class DirectApiController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string> { "some data", "more data", "loads of data" };
        }
    }
}
