using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlazorHosted.Server.Controllers
{
    // orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet("Login")]
        public ActionResult Login(string returnUrl)
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "/"
            });
        }

        [Authorize]
        [CsrfProtectionCorsPreflight]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
