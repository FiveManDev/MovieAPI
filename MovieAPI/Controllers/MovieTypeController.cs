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
    public class MovieTypeController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly MovieAPIDbContext context;
        private readonly ILogger<MovieTypeController> logger;
        public MovieTypeController(IMapper mapper, MovieAPIDbContext context, ILogger<MovieTypeController> logger)
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
                var movieTypes = context.MovieTypes.ToList();
                if (movieTypes == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod().Name.GetDataError("MovieType", "Movie Type is empty"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message= "Movie Type is empty"
                    });
                }
                var movieTypeDTOs = mapper.Map<List<MovieType>, List<MovieTypeDTO>>(movieTypes);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieType",movieTypes.Count()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all movie type success",
                    Data = movieTypeDTOs
                });
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieType", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
