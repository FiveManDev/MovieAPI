using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using System.Reflection;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class GenreController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly MovieAPIDbContext context;
        private readonly ILogger<GenreController> logger;
        public GenreController(IMapper mapper, MovieAPIDbContext context, ILogger<GenreController> logger)
        {
            this.mapper = mapper;
            this.context = context;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var genres = context.Genres.ToList();
                if (genres == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod().Name.GetDataError("Genre", ""));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Genres is empty!"
                    });
                }
                var genreDTOs = mapper.Map<List<Genre>, List<GenreDTO>>(genres);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Genre", genres.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all genre success",
                    Data = genreDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Genre", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
