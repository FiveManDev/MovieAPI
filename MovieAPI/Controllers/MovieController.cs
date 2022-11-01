using Amazon.Auth.AccessControlPolicy;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Models.Pagination;
using MovieAPI.Services.AWS;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> logger;
        private readonly IMapper _mapper;
        private readonly MovieAPIDbContext _db;
        private readonly IAmazonS3 s3Client;

        public MovieController(ILogger<MovieController> iLogger, IMapper mapper, MovieAPIDbContext db, IAmazonS3 amazonS3)
        {
            logger = iLogger;
            _mapper = mapper;
            _db = db;
            s3Client = amazonS3;

        }
        [HttpGet]
        public IActionResult GetMovieInformationById([Required] Guid id)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movie = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .FirstOrDefault(movie => movie.MovieID == id);

                if (movie == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformation", "Movie Not Found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movie Not Found"
                    });
                }

                var movieDTO = _mapper.Map<MovieInformation, MovieDTO>(movie);
                movieDTO = calculateRating(movieDTO);
                movieDTO = getGenreName(movieDTO);
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("Profile", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Movie Information By Id",
                    Data = movieDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMovieSortByPublicationTime()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .OrderByDescending(movie => movie.PublicationTime)
                    .ToList();
                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformation", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Movie Sort By Publication Time Success",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetTopLatestReleaseMovies(int top = 0)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                if (top <= 0) top = 6;
                if (top > 10) top = 10;

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .OrderByDescending(movie => movie.ReleaseTime)
                    .Take(top).ToList();

                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformations", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Top Latest Release Movies Success",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetAllMovieIsPremium()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());


                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .Where(movie => string.Equals(movie.Classification.ClassName, "Premium"))
                    .ToList();

                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformations", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get All Movie Is Premium Success",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetTopLatestPublicationMovies(int top = 0)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                if (top <= 0) top = 6;
                if (top > 10) top = 10;

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .OrderByDescending(movie => movie.PublicationTime)
                    .Take(top).ToList();

                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformation", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformation", movies.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMoviesBasedOnType([Required] Guid typeId, int top = 0)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                if (top <= 0) top = 6;
                if (top > 10) top = 10;

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .Where(movie => movie.MovieTypeID == typeId)
                    .Take(top).ToList();

                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformation", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformation", movies.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get movies based on type success",
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMoviesBasedOnGenre([Required] Guid genreID, int top = 0)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movieGenreInformations = _db.MovieGenreInformations
                    .Include(mg => mg.MovieInformation)
                    .Include(mg => mg.MovieInformation.User.Profile)
                    .Include(mg => mg.MovieInformation.Classification)
                    .Include(mg => mg.MovieInformation.MovieType)
                    .Where(mg =>
                    mg.GenreID == genreID
                    && mg.MovieInformation.IsVisible != false)
                    .Select(mg => mg.MovieInformation)
                    .Take(top)
                    .ToList();
                if (movieGenreInformations.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformation", "Movies Not Found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movies Not Found"
                    });
                }
                var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movieGenreInformations);
                movieDTOs.ForEach(movieDTO =>
                {
                    movieDTO = calculateRating(movieDTO);
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformation", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMovieBaseOnTopRating(int top = 0)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movies = new List<MovieInformation>();
                var listID = getTopRating(top);
                foreach (var dTO in listID)
                {
                    var movie = _db.MovieInformations
                    .Include(m => m.User.Profile)
                    .Include(m => m.Classification)
                    .Include(m => m.MovieType)
                    .Include(m => m.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .SingleOrDefault(m => m.MovieID == dTO.MovieID);
                    movies.Add(movie);
                }
                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformations", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Top Latest Release Movies Success",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMovieBaseOnFilter(Guid genreID, string quality, float ratingMin, float ratingMax, int releaseTimeMin, int releaseTimeMax)
        {
            try
            {
                if (genreID == Guid.Empty && quality == null && ratingMin == 0 && ratingMax == 0 && releaseTimeMin == 0 && releaseTimeMax == 0)
                {
                    var movie = _db.MovieInformations
                        .Include(m => m.User.Profile)
                        .Include(m => m.Classification)
                        .Include(m => m.MovieType)
                        .Include(m => m.MovieGenreInformations)
                        .ToList();
                    var moviesDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movie);
                    moviesDTOs.ForEach(movieDTO =>
                    {
                        movieDTO = calculateRating(movieDTO);
                        movieDTO = getGenreName(movieDTO);
                    });
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Get Movie Base On Filter Success",
                        Data = moviesDTOs
                    });
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var listTopRating = _db.Reviews.Select(r => r.MovieID).Distinct().ToList();
                var listmovieID = new List<MovieDTO>();
                foreach (var item in listTopRating)
                {
                    var movieDto = new MovieDTO();
                    movieDto.MovieID = item;
                    listmovieID.Add(calculateRating(movieDto));
                }

                var listID = listmovieID.Where(movieDTO => movieDTO.Rating >= ratingMin
                 && movieDTO.Rating <= ratingMax).ToList();
                if (listID.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movies Not Found"
                    });
                }
                var movies = new List<MovieInformation>();
                foreach (var dTO in listID)
                {
                    var movie = _db.MovieInformations
                    .Include(m => m.User.Profile)
                    .Include(m => m.Classification)
                    .Include(m => m.MovieType)
                    .Include(m => m.MovieGenreInformations)
                    .SingleOrDefault(m => m.MovieID == dTO.MovieID
                    && m.Quality.Equals(quality)
                    && m.ReleaseTime.Year >= releaseTimeMin
                    && m.ReleaseTime.Year <= releaseTimeMax
                    );
                    if (movie == null)
                    {
                        continue;
                    }
                    var check = movie.MovieGenreInformations.Any(mg => mg.GenreID == genreID);
                    if (check) movies.Add(movie);
                }
                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformations", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Top Latest Release Movies Success",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMovieSortByRating()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movies = new List<MovieInformation>();
                var listID = getTopRating(0);
                foreach (var dTO in listID)
                {
                    var movie = _db.MovieInformations
                    .Include(m => m.User.Profile)
                    .Include(m => m.Classification)
                    .Include(m => m.MovieType)
                    .Include(m => m.MovieGenreInformations)
                    .Where(movie => movie.IsVisible != false)
                    .SingleOrDefault(m => m.MovieID == dTO.MovieID);
                    if (movie == null) movies.Add(movie);
                }
                if (movies.Count == 0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("MovieInformations", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformations", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Movie Base On Filter Success",
                    Data = movieDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        // Get Movies Based On Search Text
        [HttpGet]
        public IActionResult GetMoviesBasedOnSearchText(string searchText, int top = 0)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                if (top <= 0) top = 6;
                if (top > 10) top = 10;

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.MovieName.Contains(searchText)
                        || movie.Description.Contains(searchText)
                        || movie.Actor.Contains(searchText)
                        || movie.Director.Contains(searchText)
                        || movie.ReleaseTime.ToString().Contains(searchText))
                    .Take(top).ToList();

                if (movies.Count == 0)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformatio", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = movieDTOs.Count() == 0 ? "Empty!" : "",
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetMoviesBasedOnSearchTextInCatalog(string searchText)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.MovieName.Contains(searchText)
                        || movie.Description.Contains(searchText)
                        || movie.Actor.Contains(searchText)
                        || movie.Director.Contains(searchText)
                        || movie.ReleaseTime.ToString().Contains(searchText))
                    .ToList();

                if (movies.Count == 0)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
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
                    movieDTO = getGenreName(movieDTO);
                });
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformatio", movieDTOs.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = movieDTOs.Count() == 0 ? "Empty!" : "",
                    Data = movieDTOs
                });

            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetTotalNumberOfMovies()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var count = _db.MovieInformations.Count();
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("MovieInformation", count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = count == 0 ? "Empty!" : "",
                    Data = count
                });

            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostMovie([FromForm] PostMovieModel postMovieModel)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                if(postMovieModel.Thumbnail==null|| postMovieModel.CoverImage==null || postMovieModel.Movie == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess=false,
                        Message = "File is null"
                    });
                }
                var thumbnail = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Thumbnail, EnumObject.FileType.Image);
                if (thumbnail == "")
                {
                    throw new Exception("Server error");
                }
                var coverImage = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.CoverImage, EnumObject.FileType.Image);
                if (coverImage == "")
                {
                    throw new Exception("Server error");
                }
                var movieURL = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Movie, EnumObject.FileType.Video);
                if (movieURL == "")
                {
                    throw new Exception("Server error");
                }
                var ClassID = _db.Classifications.SingleOrDefault(cls => cls.ClassName == postMovieModel.ClassName).ClassID;
                var MovieTypeID = _db.MovieTypes.SingleOrDefault(mt => mt.MovieTypeName == postMovieModel.MovieTypeName).MovieTypeID;
                var movieInformation = new MovieInformation
                {
                    MovieName = postMovieModel.MovieName,
                    Description = postMovieModel.Description,
                    Thumbnail = thumbnail,
                    Country = postMovieModel.Country,
                    Actor = postMovieModel.Actor.ToString(),
                    Director = postMovieModel.Director,
                    Language = postMovieModel.Language,
                    Subtitle = postMovieModel.Subtitle,
                    ReleaseTime = postMovieModel.ReleaseTime,
                    PublicationTime = postMovieModel.PublicationTime,
                    CoverImage = coverImage,
                    Age = postMovieModel.Age,
                    MovieURL = movieURL,
                    RunningTime = postMovieModel.RunningTime,
                    Quality = postMovieModel.Quality,
                    UserID = postMovieModel.UserID,
                    ClassID = ClassID,
                    MovieTypeID = MovieTypeID,
                };
                _db.MovieInformations.Add(movieInformation);
                var returnValue = _db.SaveChanges();
                var GenreIDs = new List<Guid>();
                foreach (var genreName in postMovieModel.GenreName)
                {
                    GenreIDs.Add(_db.Genres.SingleOrDefault(g => g.GenreName == genreName).GenreID);
                }
                if (returnValue != 0)
                {
                    List<MovieGenreInformation> movieGenreInformation = new List<MovieGenreInformation>();
                    if (GenreIDs.Count != 0)
                    {
                        foreach (var genreID in GenreIDs)
                        {
                            movieGenreInformation.Add(new MovieGenreInformation
                            {
                                GenreID = genreID,
                                MovieID = movieInformation.MovieID
                            });
                        }
                        _db.AddRange(movieGenreInformation);
                        var checkValue = _db.SaveChanges();
                        if (checkValue == 0)
                        {
                            throw new Exception("Save data of Movie Genre Information failed");
                        }
                        logger.LogInformation(MethodBase.GetCurrentMethod().Name.PostDataSuccess("MovieInformation"));
                        return Ok(new ApiResponse
                        {
                            IsSuccess = true,
                            Message = "Upload movie information success"
                        });
                    }
                }
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateMovie([FromForm] PostMovieModel postMovieModel)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movieInformation = _db.MovieInformations.Find(postMovieModel.MovieID);
                var thumbnail = movieInformation.Thumbnail;
                var coverImage = movieInformation.CoverImage;
                var movieURL = movieInformation.MovieURL;
                if (postMovieModel.Thumbnail != null)
                {
                    thumbnail = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Thumbnail, EnumObject.FileType.Image);
                }
                if (postMovieModel.CoverImage != null)
                {
                    coverImage = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.CoverImage, EnumObject.FileType.Image);
                }
                if (postMovieModel.Movie != null)
                {
                    movieURL = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Movie, EnumObject.FileType.Video);
                }
                var ClassID = _db.Classifications.SingleOrDefault(cls => cls.ClassName == postMovieModel.ClassName).ClassID;
                var MovieTypeID = _db.MovieTypes.SingleOrDefault(mt => mt.MovieTypeName == postMovieModel.MovieTypeName).MovieTypeID;
                movieInformation.MovieName = postMovieModel.MovieName;
                movieInformation.Description = postMovieModel.Description;
                movieInformation.Thumbnail = thumbnail;
                movieInformation.Country = postMovieModel.Country;
                movieInformation.Actor = postMovieModel.Actor.ToString();
                movieInformation.Director = postMovieModel.Director;
                movieInformation.Language = postMovieModel.Language;
                movieInformation.Subtitle = postMovieModel.Subtitle;
                movieInformation.ReleaseTime = postMovieModel.ReleaseTime;
                movieInformation.PublicationTime = postMovieModel.PublicationTime;
                movieInformation.CoverImage = coverImage;
                movieInformation.Age = postMovieModel.Age;
                movieInformation.MovieURL = movieURL;
                movieInformation.RunningTime = postMovieModel.RunningTime;
                movieInformation.Quality = postMovieModel.Quality;
                movieInformation.UserID = postMovieModel.UserID;
                movieInformation.ClassID = ClassID;
                movieInformation.MovieTypeID = MovieTypeID;
                _db.MovieInformations.Update(movieInformation);
                var returnValue = _db.SaveChanges();
                var GenreIDs = new List<Guid>();
                foreach (var genreName in postMovieModel.GenreName)
                {
                    GenreIDs.Add(_db.Genres.SingleOrDefault(g => g.GenreName == genreName).GenreID);
                }
                if (returnValue != 0)
                {
                    List<MovieGenreInformation> movieGenreInformation = new List<MovieGenreInformation>();
                    if (GenreIDs.Count != 0)
                    {
                        foreach (var genreID in GenreIDs)
                        {
                            movieGenreInformation.Add(new MovieGenreInformation
                            {
                                GenreID = genreID,
                                MovieID = movieInformation.MovieID
                            });
                        }
                        var MovieGenreInformations = _db.MovieGenreInformations.Where(mg=>mg.MovieID==movieInformation.MovieID).ToList();
                        _db.RemoveRange(MovieGenreInformations);
                        _db.AddRange(movieGenreInformation);
                        var checkValue = _db.SaveChanges();
                        if (checkValue == 0)
                        {
                            throw new Exception("Save data of Movie Genre Information failed");
                        }
                        logger.LogInformation(MethodBase.GetCurrentMethod().Name.PostDataSuccess("MovieInformation"));
                        return Ok(new ApiResponse
                        {
                            IsSuccess = true,
                            Message = "Upload movie information success"
                        });
                    }
                }
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Upload movie information failed"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult DeleteMovie(Guid movieID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var movieInformation = _db.MovieInformations.Find(movieID);
                if (movieInformation == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movie not found"
                    });
                }
                _db.MovieInformations.Remove(movieInformation);
                var returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Remove movie failed");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete movie success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult UpdateMovieStatus([FromBody] JObject data)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            try
            {
                Guid movieID = data["movieID"].ToObject<Guid>();
                bool isVisible = data["isVisible"].ToObject<bool>();
                var movie = _db.MovieInformations.Find(movieID);
                if (movie == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movie not found"
                    });
                }
                movie.IsVisible = isVisible;
                _db.MovieInformations.Update(movie);
                var returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update status movie success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetTotalMovie()
        {

            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            try
            {
                var totalMovie = _db.MovieInformations.Count();
                if (totalMovie == 0)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", "Movies Not Found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "The system doesn't have any movies"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get Total Movie success",
                    Data = totalMovie
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("MovieInformation", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        // Get Movies Based On Search Text
        [HttpGet]
        public IActionResult GetMovies([FromQuery] Pager pager, string q, string sortBy, string sortType)
        {
            try
            {
                q = (q == null) ? "" : q.Trim().ToLower();
    
                List<MovieInformation> movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.MovieName.ToLower().Contains(q)).ToList();

                if (movies.Count == 0)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Data = null
                    });
                }

                // can not map PaginatedList???
                var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movies);

                movieDTOs.ForEach(movieDTO =>
                {
                    // calcute rating and get genre name
                    movieDTO = getGenreName(movieDTO);
                    movieDTO = calculateRating(movieDTO);
                });

                sortBy = (sortBy == null) ? "date" : sortBy.Trim().ToLower();
                sortType = (sortType == null) ? "desc" : sortType.Trim().ToLower();

                if (sortBy == "date")
                {
                    if (sortType == "desc")
                    {
                        movieDTOs = movieDTOs.OrderByDescending(movie => movie.PublicationTime).ToList();
                    }
                    else if (sortType == "asc")
                    {
                        movieDTOs = movieDTOs.OrderBy(movie => movie.PublicationTime).ToList();
                    }
                }
                else if (sortBy == "rating")
                {
                    // do nothing...maybe later :D
                    if (sortType == "desc")
                    {
                        movieDTOs = movieDTOs.OrderByDescending(movie => movie.Rating).ToList();
                    }
                    else if (sortType == "asc")
                    {
                        movieDTOs = movieDTOs.OrderBy(movie => movie.Rating).ToList();
                    }
                }

                PaginatedList<MovieDTO> result = PaginatedList<MovieDTO>.ToPageList(movieDTOs, pager.pageIndex, pager.pageSize);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = new
                    {
                        movies = result,
                        pager = result.paginationDTO
                    }
                });

            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("none", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MovieDTO calculateRating(MovieDTO movieDTO)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            // calculate rating
            var rating = _db.Reviews.Where(review => review.MovieID.Equals(movieDTO.MovieID)).Sum(review => review.Rating);
            var count = _db.Reviews.Where(review => review.MovieID.Equals(movieDTO.MovieID)).Count();

            movieDTO.Rating = count == 0 ? 0 : (float)(rating / count);

            return movieDTO;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MovieDTO getGenreName(MovieDTO movieDTO)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            var genres = _db.Genres.Where(genre => movieDTO.Genres.Contains(genre.GenreID.ToString()))
                .Select(genre => genre.GenreName).ToList();
            movieDTO.Genres = genres;
            return movieDTO;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public List<MovieDTO> getTopRating(int top)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            var listTopRating = _db.Reviews.Distinct().ToList();
            var listID = new List<MovieDTO>();
            foreach (var item in listTopRating)
            {
                var movieDto = new MovieDTO();
                movieDto.MovieID = item.MovieID;
                listID.Add(calculateRating(movieDto));
            }
            if (top > 0)
            {
                return listID.OrderByDescending(movieDTO => movieDTO.Rating).Take(top).ToList();
            }
            return listID.OrderByDescending(movieDTO => movieDTO.Rating).ToList();
        }

    }
}
