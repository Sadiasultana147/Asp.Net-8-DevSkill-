using AWS.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Application
{
    public interface IS3FileService
    {
        Task<S3FileModel> UploadFileAsync(IFormFile file, string bucketName);
        Task<List<string>> ListFilesAsync(string bucketName);
        Task<Stream> DownloadFileAsync(string bucketName, string objectKey);
        Task<bool> DeleteFileAsync(string bucketName, string objectKey);
    }
}
