﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using notip_server.Dto;
using notip_server.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace notip_server.Controllers
{
    public class SystemAuthorization : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(token) )
            {
                ResponseAPI responseAPI = new ResponseAPI();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                responseAPI.Message = "Lỗi xác thực";
                context.Result = new JsonResult(responseAPI);
            }
            else
            {
                try
                {
                    string tokenValue = token.Replace("Bearer", string.Empty).Trim();
                    ClaimsPrincipal claimsPrincipal = DecodeJWT(tokenValue, EnviConfig.SecretKey);
                    context.HttpContext.Response.StatusCode = (int)(HttpStatusCode.OK);
                }
                catch (SecurityTokenExpiredException ex)
                {
                    ResponseAPI responseAPI = new ResponseAPI();
                    context.HttpContext.Response.StatusCode = responseAPI.Status = (int)HttpStatusCode.NotAcceptable;
                    responseAPI.Message = "Hết phiên đăng nhập";
                    context.Result = new JsonResult(responseAPI);
                }
                catch (Exception ex)
                {
                    ResponseAPI responseAPI = new ResponseAPI();
                    context.HttpContext.Response.StatusCode = responseAPI.Status = (int)HttpStatusCode.Unauthorized;
                    responseAPI.Message = "Lỗi xác thực";
                    context.Result = new JsonResult(responseAPI);
                }
            }
        }

        public static string GetCurrentUser(IHttpContextAccessor contextAccessor)
        {
            try
            {
                string token = contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                string tokenValue = token.Replace("Bearer", string.Empty).Trim();
                ClaimsPrincipal claimsPrincipal = DecodeJWT(tokenValue, EnviConfig.SecretKey);
                string userSession = claimsPrincipal.FindFirstValue(ClaimTypes.Sid);

                return userSession;
            } catch
            {
                throw new Exception("Lỗi xác thực");
            }
        }

        public static ClaimsPrincipal DecodeJWT(string token, string secretAuthkey)
        {
            var key = Encoding.UTF8.GetBytes(secretAuthkey);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);

            return claims;
        }
    }
}
