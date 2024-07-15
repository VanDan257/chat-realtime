using notip_server.Interfaces;
using notip_server.Service;
using notip_server.Utils;
using System.Runtime.CompilerServices;

namespace notip_server.Extensions
{
    public static class EmailServiceExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            var mailSettings = configuration.GetSection("MailSetting");

            services.Configure<MailSetting>(mailSettings);
            services.AddTransient<ISendMailService, SendMailService>();

            return services;
        }
    }
}
