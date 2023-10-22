using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SQS;
using AWS.Application;
using AWS.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Infrastructure
{
    public class S3FileService : IS3FileService
    {
        private readonly IAmazonS3 _s3Client;

        public S3FileService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        public S3FileService()
        {
            _s3Client= new AmazonS3Client();
        }

        public async Task<List<string>> ListFilesAsync(string bucketName)
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
            };

            var fileNames = new List<string>();

            do
            {
                var response = await _s3Client.ListObjectsV2Async(listObjectsRequest);
                foreach (var s3Object in response.S3Objects)
                {
                    fileNames.Add(s3Object.Key);
                }

                listObjectsRequest.ContinuationToken = response.NextContinuationToken;
            } while (listObjectsRequest.ContinuationToken != null);

            return fileNames;
        }
        public async Task<S3FileModel> UploadFileAsync(IFormFile file, string bucketName)
        {
            var fileKey = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fileUrl = $"https://{bucketName}.s3.amazonaws.com/{fileKey}";

            using (var fileTransferUtility = new TransferUtility(_s3Client))
            {
                await fileTransferUtility.UploadAsync(file.OpenReadStream(), bucketName, fileKey);
            }

            return new S3FileModel
            {
                FileName = file.FileName,
                FileUrl = fileUrl
            };
        }
        public async Task<Stream> DownloadFileAsync(string bucketName, string objectKey)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
            };

            using (GetObjectResponse response = await _s3Client.GetObjectAsync(getObjectRequest))
            {
                using (Stream responseStream = response.ResponseStream)
                {
                    var memoryStream = new MemoryStream();
                    await responseStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin); 
                    return memoryStream;
                }
            }
        }
        public async Task<bool> DeleteFileAsync(string bucketName, string objectKey)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
            };

            try
            {
                await _s3Client.DeleteObjectAsync(deleteObjectRequest);
                return true; // File deleted successfully
            }
            catch (Exception)
            {
                return false; // Error occurred while deleting the file
            }
        }
    }
}
