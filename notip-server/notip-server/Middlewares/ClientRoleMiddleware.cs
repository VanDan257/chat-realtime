using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using notip_server.Interfaces;
using notip_server.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace notip_server.Middlewares
{
    public class ClientRoleMiddleware : IAsyncActionFilter
    {
        private readonly IJwtService _jwtService;

        public ClientRoleMiddleware(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if(string.IsNullOrEmpty(token) )
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                string tokenValue = token.Replace("Bearer", string.Empty).Trim();
                ClaimsPrincipal claimsPrincipal = _jwtService.DecodeJWT(tokenValue, EnviConfig.SecretKey);
                string role = claimsPrincipal.FindFirstValue(ClaimTypes.Role);

                if(role == Constants.Role.CLIENT || role == Constants.Role.ADMIN || role == Constants.Role.MODERATOR)
                {
                    await next();
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
