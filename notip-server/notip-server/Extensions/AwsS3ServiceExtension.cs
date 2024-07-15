using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using notip_server.Utils;

namespace notip_server.Extensions
{
    public static class AwsS3ServiceExtension
    {
        public static IServiceCollection AddAwsS3Services(this IServiceCollection services, IConfiguration configuration)
        {

            // Cấu hình AWS S3
            var awsOptions = configuration.GetAWSOptions();
            awsOptions.Credentials = new Amazon.Runtime.BasicAWSCredentials(
                EnviConfig.AccessKeyAwsS3, 
                EnviConfig.ServiceURLAwsS3);
            awsOptions.Region = RegionEndpoint.EUWest3; // Chỉnh theo region của Cellar

            // Khởi tạo bucket nếu chưa tồn tại
            //bucketInitializer.InitializeBucketAsync().Wait();

            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();
            //services.AddSingleton<S3BucketInitializer>();

            return services;
        }
    }
}