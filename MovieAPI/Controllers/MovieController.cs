using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.DbConfig;
using MovieAPI.Data;
using MovieAPI.Models;
using System.Reflection;
using MovieAPI.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models.DTO;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IMapper _mapper;
        private readonly MovieAPIDbContext _db;
        public MovieController(ILogger<UserController> iLogger, IMapper mapper, MovieAPIDbContext db)
        {
            logger = iLogger;
            _mapper = mapper;
            _db = db;
        }

        // Get a movie information by id
        [HttpGet]
        public IActionResult GetAMovieInformationById(string id)
        {
            try
            {
                
                var movie = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    //.Include(movie => movie.Genre)
                    .FirstOrDefault(movie => movie.MovieID.ToString() == id);
                if (movie != null)
                {
                    var movieDTO = _mapper.Map<MovieInformation, MovieDTO>(movie);
                    movieDTO = calculateRating(movieDTO);
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("Profile", 1));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
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
                
                IEnumerable<Genre> genres = _db.Genres.ToList();
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

        // Get the list of 6 latest release movies
        [HttpGet]
        public IActionResult GetTopLastestReleaseMovies(int top)
        {
            try
            {
                if (top <= 0) top = 6;
                if (top > 10) top = 10;
                
                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    //.Include(movie => movie.Genre)
                    .OrderBy(movie => movie.ReleaseTime)
                    .Take(top).ToList();

                if (movies == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movies Not Found"
                    });
                }

                var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movies);
                movieDTOs.ForEach(movieDTO =>
                {
                    movieDTO = calculateRating(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = movieDTOs
                });
          
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }

        // Get the list of 6 latest publication movies
        [HttpGet]
        public IActionResult GetTopLastestPublicationMovies(int top)
        {
            try
            {
                if (top <= 0) top = 6;
                if (top > 10) top = 10;
                
                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    //.Include(movie => movie.Genre)
                    .OrderBy(movie => movie.PublicationTime)
                    .Take(top).ToList();

                if (movies == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movies Not Found"
                    });
                }

                var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movies);
                movieDTOs.ForEach(movieDTO =>
                {
                    movieDTO = calculateRating(movieDTO);
                });
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }

        // Get Movies Based On Type
        [HttpGet]
        public IActionResult GetMoviesBasedOnType(string type, int top)
        {
            try
            {
                if (top <= 0) top = 6;
                if (top > 10) top = 10;
                
                var typeId = _db.MovieTypes.FirstOrDefault(movieType => movieType.MovieTypeName.Equals(type))?.MovieTypeID; 

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    //.Include(movie => movie.Genre)
                    .Where(movie => movie.MovieTypeID == typeId)
                    .Take(top).ToList();

                if (movies == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movies Not Found"
                    });
                }

                var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movies);
                movieDTOs.ForEach(movieDTO =>
                {
                    movieDTO = calculateRating(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = movieDTOs.Count() == 0 ? "Empty!" : "",
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }

        // Get Movies Based On Search Text
        [HttpGet]
        public IActionResult GetMoviesBasedOnSearchText(string searchText, int top)
        {
            try
            {
                if (top <= 0) top = 6;
                if (top > 10) top = 10;
                
                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    //.Include(movie => movie.Genre)
                    .Where(movie => movie.MovieName.Contains(searchText)
                        || movie.Description.Contains(searchText)
                        || movie.Actor.Contains(searchText)
                        || movie.Director.Contains(searchText))
                    .Take(top).ToList();

                if (movies == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movies Not Found"
                    });
                }

                var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movies);
                movieDTOs.ForEach(movieDTO =>
                {
                    movieDTO = calculateRating(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = movieDTOs.Count() == 0 ? "Empty!" : "",
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }

        // Total number of movies
        [HttpGet]
        public IActionResult GetTotalNumberOfMovies()
        {
            try
            {
                var count = _db.MovieInformations.Count();

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = count == 0 ? "Empty!" : "",
                    Data = count
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MovieDTO calculateRating(MovieDTO movieDTO)
        {
            // calculate rating
            var rating = _db.Reviews.Where(review => review.MovieID.Equals(movieDTO.MovieID)).Sum(review => review.Rating);
            var count = _db.Reviews.Where(review => review.MovieID.Equals(movieDTO.MovieID)).Count();
            
            movieDTO.Rating = count == 0 ? 0 : (float)(rating / count);

            return movieDTO;
        }
       
    }
}
