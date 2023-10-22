using AWS.Application;
using AWS.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AWS.WEB.Controllers
{
    public class S3FileController : Controller
    {
        private static readonly string BucketName = "sadia-aspnet-b8"; 
        private readonly IS3FileService _s3Service;

        public S3FileController(IS3FileService s3Service)
        {
            _s3Service = s3Service;
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            
            var uploadedFile = await _s3Service.UploadFileAsync(file, BucketName);
            if (uploadedFile != null)
            {
                return RedirectToAction("ListFiles");
            }
            else
            {
                
                return View("ErrorView"); 
            }
        }

        public IActionResult ListFiles()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ListFilesPost()
        {
            var fileNames = await _s3Service.ListFilesAsync(BucketName);

            var fileList = fileNames.Select(fileName => new S3FileModel { FileName = fileName });

            return Json(new
            {
                data = fileList
            });
        }


        public async Task<IActionResult> DownloadFile(string objectKey)
        {
            var fileStream = await _s3Service.DownloadFileAsync(BucketName, objectKey);

            if (fileStream == null)
            {
                return NotFound(); 
            }

           
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + objectKey);

            return File(fileStream, "application/octet-stream");
        }

        public async Task<IActionResult> DeleteFile(string objectKey)
        {
            var success = await _s3Service.DeleteFileAsync(BucketName, objectKey);

            if (!success)
            {
                return BadRequest("Failed to delete the file.");
            }

            return RedirectToAction("ListFiles");
        }
    }
}
