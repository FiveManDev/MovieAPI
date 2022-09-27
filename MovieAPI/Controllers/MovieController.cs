using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.DbConfig;
using MovieAPI.Data;
using MovieAPI.Models;
using System.Reflection;
using MovieAPI.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IMapper _mapper;
        public MovieController(ILogger<UserController> iLogger, IMapper mapper)
        {
            logger = iLogger;
            _mapper = mapper;
        }

        // Get a movie information by id
        [HttpGet]
        public IActionResult GetAMovieInformationById(string id)
        {
            try
            {
                using var context = new MovieAPIDbContext();
                var movie = context.MovieInformations!
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    //.Include(movie => movie.MovieType)
                    .Include(movie => movie.Genre)
                    .FirstOrDefault(movie => movie.MovieID.ToString() == id);
                if (movie != null)
                {
                    var movieDTO = _mapper.Map<MovieInformation, MovieDTO>(movie);
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("Profile", 1));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Create Account Success",
                        Data = movieDTO
                    });
                }
                else
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movie Not Found"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("MovieInformation", ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movie Not Found"
                });
            }
        }

        // Get all genre of movie
        [HttpGet]
        public IActionResult GetAllGenreOfMovie()
        {
            try
            {
                using var context = new MovieAPIDbContext();
                IEnumerable<Genre> genres = context.Genres.ToList();
                if (genres != null)
                {
                    List<String> genresName = genres.Select(genre => genre.GenreName).ToList();

                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("Genre", genresName.Count()));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Get All Genre Of Movie Success",
                        Data = genresName
                    });
                }
                else
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Cannot Get All Genre Of Movie! Something wrong!"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("Genre", ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Cannot Get All Genre Of Movie! Something wrong!"
                });
            }
        }
    
    }
}
