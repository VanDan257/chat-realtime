using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using notip_server.Interfaces;
using notip_server.Utils;

namespace notip_server.Service
{
    public class AwsS3Service : IAwsS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsS3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["Cellar:BucketName"];
        }

        public async Task UploadBlobFile(IFormFile file, string filePath)
        {
            try
            {
                if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

                var fileTransferUtility = new TransferUtility(_s3Client);
;
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = file.OpenReadStream(),
                        BucketName = "NotipCloud",
                        Key = "/" + filePath + "/" + file.FileName,
                        ContentType = file.ContentType
                    };

                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra! Hãy thử lại!");
            }
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