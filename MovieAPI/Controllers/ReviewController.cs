﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Models.Pagination;
using MovieAPI.Services.Attributes;
using MovieAPI.Services.SignalR;
using System.Reflection;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class ReviewController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly IHubContext<ReviewHub> hub;
        private readonly IMapper mapper;
        private readonly ILogger<ReviewController> logger;
        public ReviewController(MovieAPIDbContext db, IHubContext<ReviewHub> hub, IMapper mapper, ILogger<ReviewController> logger)
        {
            context = db;
            this.hub = hub;
            this.mapper = mapper;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult GetAllReviewsOfUser(Guid UserID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var listReviewData = context.Reviews
                    .Include(r => r.User.Profile)
                    .Include(r => r.MovieInformation)
                    .Where(r => r.UserID == UserID).ToList();
                var listReview = mapper.Map<List<Review>, List<ReviewDTO>>(listReviewData);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Review", listReviewData.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all review by moviewID success",
                    Data = listReview
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetAllReviewsOfMovie(Guid MovieID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var listReviewData = context.Reviews
                    .Include(r => r.User.Profile)
                    .Include(r => r.MovieInformation)
                    .Where(r => r.MovieID == MovieID).ToList();
                var listReview = mapper.Map<List<Review>, List<ReviewDTO>>(listReviewData);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Review", listReviewData.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all review by moviewID success",
                    Data = listReview
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult SearchReview(string text)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var reviews = context.Reviews
                    .Include(review => review.User.Profile)
                    .Include(review => review.MovieInformation)
                    .Where(review => review.User.Profile.FirstName.Contains(text)
                    || review.User.Profile.LastName.Contains(text)
                    || review.MovieInformation.MovieName.Contains(text)
                    || review.ReviewContent.Contains(text))
                    .ToList();
                var reviewDTO = mapper.Map<List<Review>, List<ReviewDTO>>(reviews);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Profile", reviews.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = reviewDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetReviewSortByDateCreate()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var reviews = context.Reviews
                    .Include(review => review.User.Profile)
                    .Include(review => review.MovieInformation)
                    .OrderByDescending(review => review.ReviewTime)
                    .ToList();
                var reviewDTO = mapper.Map<List<Review>, List<ReviewDTO>>(reviews);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Reviews", reviews.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get top lastest review time",
                    Data = reviewDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetReviewSortByRating()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var reviews = context.Reviews
                    .Include(review => review.User.Profile)
                    .Include(review => review.MovieInformation)
                    .OrderByDescending(review => review.Rating)
                    .ToList();
                var reviewDTO = mapper.Map<List<Review>, List<ReviewDTO>>(reviews);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Reviews", reviews.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get top lastest review time",
                    Data = reviewDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetTopLastestReview(int top)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var reviews = context.Reviews
                    .Include(review => review.User.Profile)
                    .Include(review => review.MovieInformation)
                    .OrderByDescending(review => review.ReviewTime)
                    .Take(top).ToList();
                var reviewDTO = mapper.Map<List<Review>, List<ReviewDTO>>(reviews);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Reviews", reviews.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get top lastest review time",
                    Data = reviewDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [UserBanned]
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] PostReviewModel reviewDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var review = new Review
                {
                    Title = reviewDTO.Title,
                    ReviewContent = reviewDTO.ReviewContent,
                    Rating = reviewDTO.Rating,
                    ReviewTime = DateTime.Now,
                    UserID = reviewDTO.UserID,
                    MovieID = reviewDTO.MovieID
                };
                context.Reviews.Add(review);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of review failed");
                }
                var Profile = context.Profiles.SingleOrDefault(pro => pro.UserID == reviewDTO.UserID);
                var reviewObject = new
                {
                    ReviewID= review.ReviewID,
                    Title = review.Title,
                    ReviewContent = review.ReviewContent,
                    Rating = review.Rating,
                    ReviewTime = DateTime.Now,
                    UserID = review.UserID,
                    MovieID = review.MovieID,
                    FirstName = Profile.FirstName,
                    LastName = Profile.LastName,
                    Avatar = Profile.Avatar,
                };
                await hub.Clients.Group(reviewDTO.MovieID.ToString()).SendAsync("Review", "Create",reviewDTO.UserID, reviewObject);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PostDataSuccess("Profile"));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create new review success",
                    Data = reviewObject
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [UserBanned]
        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewDTO reviewDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var review = context.Reviews.FirstOrDefault(x => x.ReviewID == reviewDTO.ReviewID);
                if (review == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.DeleteDataError("Review", "Review not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Review not found"
                    });
                }
                review.Title = reviewDTO.Title;
                review.ReviewContent = reviewDTO.ReviewContent;
                review.Rating = reviewDTO.Rating;
                review.ReviewTime = DateTime.Now;

                context.Reviews.Update(review);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of review failed");
                }
                var Profile = context.Profiles.SingleOrDefault(pro => pro.UserID == review.UserID);
                var reviewObject = new
                {
                    ReviewID= review.ReviewID,
                    Title = review.Title,
                    ReviewContent = review.ReviewContent,
                    Rating = review.Rating,
                    ReviewTime = review.ReviewTime,
                    UserID = review.UserID,
                    MovieID = review.MovieID,
                    FirstName = Profile.FirstName,
                    LastName = Profile.LastName,
                    Avatar = Profile.Avatar,
                };
                await hub.Clients.Group(review.MovieID.ToString()).SendAsync("Review", "Update",reviewDTO.UserID, reviewObject);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PutDataSuccess("Review", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update review success",
                    Data = reviewObject
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PutDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [UserBanned]
        [HttpDelete]
        public async Task<IActionResult> DeleteReview(Guid ReviewID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var review = context.Reviews.SingleOrDefault(r => r.ReviewID == ReviewID);
                if (review == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.DeleteDataError("Review", "Review not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Review not found"
                    });
                }
                context.Reviews.Remove(review);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Delete review failed");
                }
                await hub.Clients.Group(review.MovieID.ToString()).SendAsync("Review", "Delete",review.ReviewID, ReviewID);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.DeleteDataSuccess("Review", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete review success",
                    Data = ReviewID
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.DeleteDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetReviews([FromQuery] Pager pager, string q, string sortBy, string sortType)
        {
            try
            {
                q = (q == null) ? "" : q.Trim().ToLower();

                List<Review> reviews = context.Reviews
                    .Include(review => review.User.Profile)
                    .Include(review => review.MovieInformation)
                    .Where(review => review.ReviewContent.ToLower().Contains(q)
                        || review.Title.ToLower().Contains(q)
                        || review.User.Profile.FirstName.ToLower().Contains(q)
                        || review.User.Profile.LastName.ToLower().Contains(q)
                        || review.MovieInformation.MovieName.ToLower().Contains(q)).ToList();

                if (reviews.Count == 0)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Data = null
                    });
                }

                // can not map PaginatedList???
                var reviewDTOs = mapper.Map<List<Review>, List<ReviewDTO>>(reviews);

                sortBy = (sortBy == null) ? "date" : sortBy.Trim().ToLower();
                sortType = (sortType == null) ? "desc" : sortType.Trim().ToLower();
                if (sortBy == "date")
                {
                    if (sortType == "desc")
                    {
                        reviewDTOs = reviewDTOs.OrderByDescending(review => review.ReviewTime).ToList();
                    }
                    else if (sortType == "asc")
                    {
                        reviewDTOs = reviewDTOs.OrderBy(review => review.ReviewTime).ToList();
                    }
                }
                else if (sortBy == "rating")
                {
                    if (sortType == "desc")
                    {
                        reviewDTOs = reviewDTOs.OrderByDescending(review => review.Rating).ToList();
                    }
                    else if (sortType == "asc")
                    {
                        reviewDTOs = reviewDTOs.OrderBy(review => review.Rating).ToList();
                    }
                }

                PaginatedList<ReviewDTO> result = PaginatedList<ReviewDTO>.ToPageList(reviewDTOs, pager.pageIndex, pager.pageSize);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = new
                    {
                        reviews = result,
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
    }
}
