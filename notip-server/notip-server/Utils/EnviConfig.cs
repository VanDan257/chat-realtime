namespace notip_server.Utils
{
    public class EnviConfig
    {
        public static string DbConnectionString { get; private set; }
        public static string BlobConnectionString { get; private set; }
        public static string SecretKey { get; private set; }
        public static int ExpirationInMinutes { get; private set; }
        public static string DailyToken { get; private set; }
        public static string ServiceURLAwsS3 { get; private set; }
        public static string AccessKeyAwsS3 { get; private set; }
        public static string SecretKeyAwsS3 { get; private set; }
        public static string BucketNameAwsS3 { get; private set; }
        public static string Mail { get; set; }
        public static string DisplayName { get; set; }
        public static string Password { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }

        public static void Config(IConfiguration configuration)
        {
            DbConnectionString = configuration.GetConnectionString("DbConnection");
            BlobConnectionString = configuration.GetConnectionString("BlobConnectionString");
            SecretKey = configuration["JwtConfig:SecretKey"];
            ExpirationInMinutes = Convert.ToInt32(configuration["JwtConfig:ExpirationInMinutes"]);
            DailyToken = configuration["DailyToken"];
            ServiceURLAwsS3 = configuration["AWS:ServiceURL"];
            AccessKeyAwsS3 = configuration["AWS:AccessKey"];
            SecretKeyAwsS3 = configuration["AWS:SecretKey"];
            BucketNameAwsS3 = configuration["AWS:BucketName"];
            Mail = configuration["MailSetting:Mail"];
            DisplayName = configuration["MailSetting:DisplayName"];
            Password = configuration["MailSetting:Password"];
            Host = configuration["MailSetting:Host"];
            Port = int.Parse(configuration["MailSetting:Port"]);

        }
    }
}
