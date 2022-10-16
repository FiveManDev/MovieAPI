﻿using Microsoft.AspNetCore.Http;
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
using Microsoft.AspNetCore.Authorization;
using MovieAPI.Services.AWS;
using Amazon.S3;
using System.Net.WebSockets;

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
        private readonly IAmazonS3 s3Client;
        public MovieController(ILogger<UserController> iLogger, IMapper mapper, MovieAPIDbContext db, IAmazonS3 amazonS3)
        {
            logger = iLogger;
            _mapper = mapper;
            _db = db;
            s3Client = amazonS3;
        }

        // Get a movie information by id
        [HttpGet]
        // [ActionName("movie-information")]
        public IActionResult GetMovieInformationById(string id)
        {
            try
            {
                var movie = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .FirstOrDefault(movie => movie.MovieID.ToString() == id);

                if (movie == null)
                {
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
                    Data = movieDTO
                });
            }
            catch
            {
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movie Not Found"
                });
            }
        }
        [HttpGet]
        public IActionResult GetMovieSortByPublicationTime()
        {
            try
            {
                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
                    .OrderBy(movie => movie.PublicationTime)
                    .ToList();
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
                    movieDTO = getGenreName(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = movieDTOs
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }

        // Get the list of 6 latest release movies
        [HttpGet]
        // [ActionName("top-release")]
        public IActionResult GetTopLatestReleaseMovies(int top)
        {
            try
            {
                if (top <= 0) top = 6;
                if (top > 10) top = 10;

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
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
                    movieDTO = getGenreName(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = movieDTOs
                });

            }
            catch
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
        // [ActionName("top-public")]
        public IActionResult GetTopLatestPublicationMovies(int top)
        {
            try
            {
                if (top <= 0) top = 6;
                if (top > 10) top = 10;

                var movies = _db.MovieInformations
                    .Include(movie => movie.User.Profile)
                    .Include(movie => movie.Classification)
                    .Include(movie => movie.MovieType)
                    .Include(movie => movie.MovieGenreInformations)
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
                    movieDTO = getGenreName(movieDTO);
                });
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = movieDTOs
                });

            }
            catch
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
                    .Include(movie => movie.MovieGenreInformations)
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
                    movieDTO = getGenreName(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = movieDTOs.Count() == 0 ? "Empty!" : "",
                    Data = movieDTOs
                });

            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }
        [HttpGet]
        public IActionResult GetMoviesBasedOnGenre(string genreName, int top)
        {
            try
            {
                var genreID = _db.Genres.SingleOrDefault(genre => genre.GenreName.Equals(genreName)).GenreID;
                var movieGenreInformations = _db.MovieGenreInformations
                    .Include(mg => mg.MovieInformation)
                    .Include(mg => mg.MovieInformation.User.Profile)
                    .Include(mg => mg.MovieInformation.Classification)
                    .Include(mg => mg.MovieInformation.MovieType)
                    .Where(mg => mg.GenreID == genreID)
                    .Select(mg => mg.MovieInformation)
                    .Take(top)
                    .ToList();
                if (movieGenreInformations == null)
                {
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
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "",
                    Data = movieDTOs
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }
        [HttpGet]
        public IActionResult GetMovieBaseOnTopRating(int top)
        {
            try
            {
                //if (top <= 0) top = 6;
                //if (top > 10) top = 10;
                //var MovieID = _db.Reviews
                //    .Select(review => review.MovieID)
                //    .Distinct()
                //    .ToList();
                //List<Guid> listID = null;
                //foreach(var movie in MovieID)
                //{
                //    var rating = _db.Reviews.Where(review => review.MovieID==movie).Sum(review => review.Rating);
                //    var count = _db.Reviews.Where(review => review.MovieID == movie).Count();
                //    listI
                //}
                //var movie = _db.Reviews
                //    .Include(review=>review.MovieInformation)
                //    .Include(re)

                //if (movies == null)
                //{
                //    return NotFound(new ApiResponse
                //    {
                //        IsSuccess = false,
                //        Message = "Movies Not Found"
                //    });
                //}
                return Ok("");
                //var movieDTOs = _mapper.Map<List<MovieInformation>, List<MovieDTO>>(movies);
                //movieDTOs.ForEach(movieDTO =>
                //{
                //    movieDTO = calculateRating(movieDTO);
                //    movieDTO = getGenreName(movieDTO);
                //});
                //return Ok(new ApiResponse
                //{
                //    IsSuccess = true,
                //    Data = movieDTOs
                //});

            }
            catch
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
                    .Include(movie => movie.MovieGenreInformations)
                    .Where(movie => movie.MovieName.Contains(searchText)
                        || movie.Description.Contains(searchText)
                        || movie.Actor.Contains(searchText)
                        || movie.Director.Contains(searchText)
                        || movie.ReleaseTime.ToString().Contains(searchText))
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
                    movieDTO = getGenreName(movieDTO);
                });

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = movieDTOs.Count() == 0 ? "Empty!" : "",
                    Data = movieDTOs
                });

            }
            catch
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
        // [ActionName("total-movie")]
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
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Movies Not Found"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostMovie([FromBody] PostMovieModel postMovieModel)
        {
            try
            {
                // var thumbnail = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Thumbnail, EnumObject.FileType.Image);
                // var coverImage = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.CoverImage, EnumObject.FileType.Image);
                // var movieURL = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Movie, EnumObject.FileType.Video);
                var thumbnail = "";
                var coverImage="";
                var movieURL="";
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
                    ClassID = postMovieModel.ClassID,
                    MovieTypeID = postMovieModel.MovieTypeID,
                };
                _db.MovieInformations.Add(movieInformation);
                var returnValue = _db.SaveChanges();
                if (returnValue != 0)
                {
                    List<MovieGenreInformation> movieGenreInformation = new List<MovieGenreInformation>();
                    if (postMovieModel.GenreID != null)
                    {
                        foreach (var genreID in postMovieModel.GenreID)
                        {
                            movieGenreInformation.Add(new MovieGenreInformation
                            {
                                GenreID = genreID,
                                MovieID = movieInformation.MovieID
                            });
                        }
                        return Ok(new ApiResponse
                        {
                            IsSuccess = true,
                            Message = "Upload movie information success"
                        });
                    }
                }
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Upload movie information failed"
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Upload movie information failed"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateMovie([FromBody] PostMovieModel postMovieModel)
        {
            try
            {
                var movieInformation = _db.MovieInformations.Find(postMovieModel.MovieID);
                // var thumbnail = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Thumbnail, EnumObject.FileType.Image);
                // var coverImage = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.CoverImage, EnumObject.FileType.Image);
                // var movieURL = await AmazonS3Bucket.UploadFile(s3Client, postMovieModel.Movie, EnumObject.FileType.Video);
                var thumbnail = "";
                var coverImage="";
                var movieURL="";
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
                movieInformation.ClassID = postMovieModel.ClassID;
                movieInformation.MovieTypeID = postMovieModel.MovieTypeID;
                _db.MovieInformations.Update(movieInformation);
                var returnValue = _db.SaveChanges();
                if (returnValue != 0)
                {
                    List<MovieGenreInformation> movieGenreInformation = new List<MovieGenreInformation>();
                    if (postMovieModel.GenreID != null)
                    {
                        foreach (var genreID in postMovieModel.GenreID)
                        {
                            movieGenreInformation.Add(new MovieGenreInformation
                            {
                                GenreID = genreID,
                                MovieID = movieInformation.MovieID
                            });
                        }
                        return Ok(new ApiResponse
                        {
                            IsSuccess = true,
                            Message = "Upload movie information success"
                        });
                    }
                }
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Upload movie information failed"
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Upload movie information failed"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult DeleteMovie([FromBody] Guid movieID)
        {
            try
            {
                var movieInformation = _db.MovieInformations.Find(movieID);
                if (movieInformation == null)
                {
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
                    throw new Exception("");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete movie success"
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult UpdateMovieStatus([FromBody] (Guid movieID, bool isVisible) parameters)
        {
            try
            {
                var movie = _db.MovieInformations.Find(parameters.movieID);
                if (movie == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Movie not found"
                    });
                }
                movie.IsVisible = parameters.isVisible;
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
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = ""
                });
            }
        }
        [HttpGet]
        public IActionResult GetTotalMovie()
        {
            try
            {
                var totalMovie = _db.MovieInformations.Count();
                if (totalMovie == 0)
                {
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
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = ""
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

        [ApiExplorerSettings(IgnoreApi = true)]
        public MovieDTO getGenreName(MovieDTO movieDTO)
        {
            var genres = _db.Genres.Where(genre => movieDTO.Genres.Contains(genre.GenreID.ToString()))
                .Select(genre => genre.GenreName).ToList();
            movieDTO.Genres = genres;
            return movieDTO;
        }

    }
}
