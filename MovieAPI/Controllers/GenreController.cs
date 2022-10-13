using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
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

        public GenreController(IMapper mapper, MovieAPIDbContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                IEnumerable<Genre> genres = context.Genres.ToList();
                if (genres == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Cannot Get All Genre! Something wrong!"
                    });
                }
                List<GenreDTO> genreDTOs = new List<GenreDTO>();
                foreach(var genre in genres)
                {
                    genreDTOs.Add(mapper.Map<Genre, GenreDTO>(genre));
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all genre success",
                    Data = genreDTOs
                });
            }
            catch
            {
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Cannot Get All Genre! Something wrong!"
                });
            }
        }
    }
}
