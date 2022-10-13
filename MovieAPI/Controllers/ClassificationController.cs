using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models.DTO;
using MovieAPI.Models;
using MovieAPI.Services.SignalR;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class ClassificationController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly IMapper mapper;
        public ClassificationController(MovieAPIDbContext db, IMapper mapper)
        {
            context = db;
            this.mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                IEnumerable<Classification> classifications = context.Classifications.ToList();
                if (classifications == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Cannot Get All classification! Something wrong!"
                    });
                }
                List<ClassificationDTO> classificationDTOs = new List<ClassificationDTO>();
                foreach (var classification in classifications)
                {
                    classificationDTOs.Add(mapper.Map<Classification, ClassificationDTO>(classification));
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all classification success",
                    Data = classificationDTOs
                });
            }
            catch
            {
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Cannot Get All classification! Something wrong!"
                });
            }
        }

    }
}
