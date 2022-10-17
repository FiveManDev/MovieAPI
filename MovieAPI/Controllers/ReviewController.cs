using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Services.SignalR;
using System.Reflection;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
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
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Review",listReviewData.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all review by moviewID success",
                    Data = listReview
                });
            }
            catch(Exception ex)
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
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Profile",reviews.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = reviewDTO
                });
            }
            catch(Exception ex)
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
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            try
            {
                return Ok("");
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDTO reviewDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var review = new Review
                {
                    Title= reviewDTO.Title,
                    ReviewContent=reviewDTO.ReviewContent,
                    Rating= reviewDTO.Rating,
                    ReviewTime =reviewDTO.ReviewTime,
                    UserID = reviewDTO.UserID,
                    MovieID = reviewDTO.UserID
                };
                context.Reviews.Add(review);
                var returnValue = context.SaveChanges();
                if (returnValue== 0)
                {
                    throw new Exception("Save data of review failed");
                }
                await hub.Clients.Group(reviewDTO.MovieID.ToString()).SendAsync("SendReview", review);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PostDataSuccess("Profile"));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create new review success",
                    Data = review.ReviewID
                });
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewDTO reviewDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var review = new Review
                {
                    ReviewID =reviewDTO.ReviewID,
                    Title = reviewDTO.Title,
                    ReviewContent = reviewDTO.ReviewContent,
                    Rating = reviewDTO.Rating,
                    ReviewTime = reviewDTO.ReviewTime,
                    UserID = reviewDTO.UserID,
                    MovieID = reviewDTO.UserID
                };
                context.Reviews.Update(review);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of review failed");
                }
                await hub.Clients.Group(reviewDTO.MovieID.ToString()).SendAsync("UpdateReview", review);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PutDataSuccess("Review",1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update review success"
                });
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PutDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteReview([FromBody] Guid ReviewID)
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
                await hub.Clients.Group(review.MovieID.ToString()).SendAsync("DeleteReview", ReviewID);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.DeleteDataSuccess("Review",1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete review success"
                });
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.DeleteDataError("Reviews", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
