using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Services.AWS;
using MovieAPI.Services.Mail;
using MovieAPI.Services.MomoPayment;
using System.Reflection;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class FilesController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<FilesController> logger;
        public FilesController(IAmazonS3 s3Client, ILogger<FilesController> _logger)
        {
            _s3Client = s3Client;
            logger = _logger;
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                var response = await AmazonS3Bucket.UploadFile(_s3Client,file, EnumObject.FileType.Video);
                return Ok(response);
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Something went wrong"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles(string bucketName, string? prefix)
        {
            
            return Ok(await AmazonS3Bucket.GetAllFiles(_s3Client));
           
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
            
          logger.LogInformation("khang");

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
        [HttpGet]
        public IActionResult TestSendMail()
        {
            MailModel mailModel = new MailModel();
            mailModel.EmailTo = "hoangkhang12789@gmail.com";
            mailModel.Subject = "Khang";
            mailModel.Body = "Khang";
            MailService.SendMail(mailModel);
            return Ok(mailModel);
        }
         [HttpGet]
        public async Task<string> TestPayment()
        {
            var amount = 1000;
            return await MomoConnection.MomoResponse("Khang", amount.ToString(), "sss");

        }
    }
    public class TestModel{
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
