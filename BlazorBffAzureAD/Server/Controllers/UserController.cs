using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BlazorHosted.Server.Services;
using BlazorHosted.Shared.Authorization;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHosted.Server.Controllers
{
    // orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private GraphApiClientService _graphApiClientService;

        public UserController(GraphApiClientService graphApiClientService)
        {
            _graphApiClientService = graphApiClientService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCurrentUser()
        {
            return Ok(User.Identity.IsAuthenticated ? CreateUserInfoAsync(User) : UserInfo.Anonymous);
        }

        private async Task<UserInfo> CreateUserInfoAsync(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return UserInfo.Anonymous;
            }

            var userInfo = new UserInfo
            {
                IsAuthenticated = true
            };

            if (claimsPrincipal.Identity is ClaimsIdentity claimsIdentity)
            {
                userInfo.NameClaimType = claimsIdentity.NameClaimType;
                userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
            }
            else
            {
                userInfo.NameClaimType = JwtClaimTypes.Name;
                userInfo.RoleClaimType = JwtClaimTypes.Role;
            }

            if (claimsPrincipal.Claims.Any())
            {
                var claims = new List<ClaimValue>();
                var nameClaims = claimsPrincipal.FindAll(userInfo.NameClaimType);
                foreach (var claim in nameClaims)
                {
                    claims.Add(new ClaimValue(userInfo.NameClaimType, claim.Value));
                }

                // Uncomment this code if you want to send additional claims to the client.
                //foreach (var claim in claimsPrincipal.Claims.Except(nameClaims))
                //{
                //    claims.Add(new ClaimValue(claim.Type, claim.Value));
                //}

                userInfo.Claims = claims;

                var data = await _graphApiClientService.GetGraphApiUser();
            }

            return userInfo;
        }
    }
}
