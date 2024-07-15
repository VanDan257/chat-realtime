using Amazon.S3.Model;
using Amazon.S3;

namespace notip_server.Utils
{
    public class S3BucketInitializer
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3BucketInitializer(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["Cellar:BucketName"];
        }

        public async Task InitializeBucketAsync()
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(_bucketName);
            if (!bucketExists)
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = _bucketName
                });
            }
        }
    }
}
