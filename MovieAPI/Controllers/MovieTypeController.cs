using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;
using MovieAPI.Models.DTO;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class MovieTypeController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly MovieAPIDbContext context;

        public MovieTypeController(IMapper mapper, MovieAPIDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var movieTypes = context.MovieTypes.ToList();
                if (movieTypes == null)
                {
                    throw new Exception("Data is null");
                }
                List<MovieTypeDTO> movieTypeDTOs = new List<MovieTypeDTO>();
                foreach (var movieType in movieTypes)
                {
                    movieTypeDTOs.Add(mapper.Map<MovieType,MovieTypeDTO>(movieType));
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all movie type success",
                    Data = movieTypeDTOs
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Get all movie type failed"
                });
            }
        }
    }
}
