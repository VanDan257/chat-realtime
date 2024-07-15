using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using notip_server.Interfaces;
using notip_server.Utils;
using System.Security.Claims;

namespace notip_server.Middlewares
{
    public class AdminRoleMiddleware : IAsyncActionFilter
    {
        private readonly IJwtService _jwtService;

        public AdminRoleMiddleware(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public override bool Equals(object? obj)
        {
            return obj is AdminRoleMiddleware middleware &&
                   EqualityComparer<IJwtService>.Default.Equals(_jwtService, middleware._jwtService);
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

                if (role == Constants.Role.ADMIN || role == Constants.Role.MODERATOR)
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
