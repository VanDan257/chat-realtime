using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using notip_server.Data;
using notip_server.Hubs;
using notip_server.Interfaces;
using notip_server.Middlewares;
using notip_server.Models;
using notip_server.Service;

namespace notip_server.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region services
            services.AddTransient<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICallService, CallService>();
            services.AddScoped<IChatBoardService, ChatBoardService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddSingleton<IAwsS3Service, AwsS3Service>();
            services.AddSingleton<IPasswordService, PasswordService>();

            #endregion

            #region middlewares
            services.AddScoped<ClientRoleMiddleware>();
            services.AddScoped<AdminRoleMiddleware>();
            services.AddScoped<ModeratorMiddleware>();

            #endregion

            return services;
        }
    }
}
