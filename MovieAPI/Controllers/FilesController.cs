using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using MovieAPI.Models;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class FilesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                var response = await AmazonS3Bucket.UploadFileVideo(file, EnumObject.FileType.Image);
                return Ok(response);
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Data = "Something went wrong"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            return Ok(await AmazonS3Bucket.GetAllFiles());
        }

        //[HttpGet]
        //public async Task<IActionResult> GetFileByKey( string key)
        //{
        //    var bucketExists = await _s3Client.DoesS3BucketExistAsync(AppSettings.AWSS3BucketName);
        //    if (!bucketExists) return NotFound($"Bucket {AppSettings.AWSS3BucketName} does not exist.");
        //    var s3Object = await _s3Client.GetObjectAsync(AppSettings.AWSS3BucketName, key);
        //    return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
        //}

        //[HttpDelete]
        //public async Task<IActionResult> DeleteFile(string bucketName, string key)
        //{
        //    var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
        //    if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
        //    await _s3Client.DeleteObjectAsync(bucketName, key);
        //    return NoContent();
        //}
        [EnableQuery()]
        [HttpGet]
        public IActionResult TestGet()
        {
            var a = new List<TestModel>();
            a.Add(new TestModel
            {
                ID = 1,
                Name = "khang"
            });
            a.Add(new TestModel
            {
                ID = 1,
                Name = "khang"
            });
            a.Add(new TestModel
            {
                ID = 1,
                Name = "khang"
            });
            return Ok(a) ; 
        }
        [HttpGet]
        public IActionResult TestGetWithParam( string name)
        {
            var a = new List<TestModel>();
            a.Add(new TestModel
            {
                ID = 1,
                Name = "khang"
            });
            a.Add(new TestModel
            {
                ID = 1,
                Name = "khang"
            });
            a.Add(new TestModel
            {
                ID = 1,
                Name = "khang"
            });
            return Ok(new ApiResponse
            {
                IsSuccess = false,
                Message = name,
                Data = a

            });
        }
        [HttpPost]
        public IActionResult TestPost( int ID, string Name)
        {
            return Ok(new ApiResponse
            {
                IsSuccess = false,
                Message = "khang",
                Data = new TestModel
                {
                    ID = ID,
                    Name = Name
                }
            }); ;
        }
        [HttpPost]
        public IActionResult TestPostWithBody(TestModel model)
        {
            return Ok(new ApiResponse
            {
                IsSuccess = false,
                Message = "khang",
                Data = new TestModel
                {
                    ID = model.ID,
                    Name = model.Name
                }
            }); ;
        }
    }
    public class TestModel{
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
