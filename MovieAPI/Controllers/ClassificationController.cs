using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models.DTO;
using MovieAPI.Models;
using System.Reflection;
using MovieAPI.Helpers;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class ClassificationController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<ClassificationController> logger;
        public ClassificationController(MovieAPIDbContext db, IMapper mapper, ILogger<ClassificationController> logger)
        {
            context = db;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var classifications = context.Classifications.ToList();
                if (classifications == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Classification", "Classification is empty"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Classification is empty"
                    });
                }
                var classificationDTOs = mapper.Map<List<Classification>,List<ClassificationDTO>>(classifications);
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("Classification",classifications.Count()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all classification success",
                    Data = classificationDTOs
                });
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Classification", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

    }
}
