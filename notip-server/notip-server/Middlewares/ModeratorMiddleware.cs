using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using notip_server.Interfaces;
using notip_server.Utils;
using System.Security.Claims;

namespace notip_server.Middlewares
{
    public class ModeratorMiddleware : IAsyncActionFilter
    {
        private readonly IJwtService _jwtService;

        public ModeratorMiddleware(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                string tokenValue = token.Replace("Bearer", string.Empty).Trim();
                ClaimsPrincipal claimsPrincipal = _jwtService.DecodeJWT(tokenValue, EnviConfig.SecretKey);
                string role = claimsPrincipal.FindFirstValue(ClaimTypes.Role);

                if (role == Constants.Role.MODERATOR)
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
