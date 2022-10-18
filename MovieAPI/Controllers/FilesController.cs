using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Services.Attributes;
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
        public async Task<IActionResult> UploadFile(IFormFile file,List<int> a)
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
        public async Task<IActionResult> GetAllFiles(string bucketName, string prefix)
        {
            
            return Ok(await AmazonS3Bucket.GetAllFiles(_s3Client));
           
        }
        [Authorize]
        [UserBanned]
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
        
    }
    public class TestModel{
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
