using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace notip_server.Interfaces
{
    public interface IAwsS3Service
    {
        Task UploadBlobFile(IFormFile file, string filePath);
    }
}