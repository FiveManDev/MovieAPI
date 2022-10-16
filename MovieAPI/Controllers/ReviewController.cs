using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Services.SignalR;

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
        public ReviewController(MovieAPIDbContext db, IHubContext<ReviewHub> hub, IMapper mapper)
        {
            context = db;
            this.hub = hub;
            this.mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAllReviewsOfUser(Guid UserID)
        {
            try
            {
                var listReviewData = context.Reviews
                    .Include(r => r.User.Profile)
                    .Include(r => r.MovieInformation)
                    .Where(r => r.UserID == UserID).ToList();
                var listReview = mapper.Map<List<Review>, List<ReviewDTO>>(listReviewData);
                
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get all review by moviewID success",
                    Data = listReview
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Get all revie by moviewID failed"
                });
            }
        }
        [HttpGet]
        public IActionResult SearchReview(string text)
        {
            try
            {
                var reviews = context.Reviews
                    .Include(review => review.User.Profile)
                    .Include(review => review.MovieInformation)
                    .Where(review => review.User.Profile.FirstName.Contains(text)
                    || review.User.Profile.LastName.Contains(text)
                    || review.MovieInformation.MovieName.Contains(text)
                    || review.ReviewContent.Contains(text))
                    .ToList();
                var reviewDTO = mapper.Map<List<Review>, List<ReviewDTO>>(reviews);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = reviewDTO
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
        public IActionResult GetTopLastestReview(int top)
        {
            try
            {
                return Ok("");
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
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDTO reviewDTO)
        {
            try
            {
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
                    throw new Exception("Create new review failed");
                }
                await hub.Clients.Group(reviewDTO.MovieID.ToString()).SendAsync("SendReview", review);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create new review success",
                    Data = review.ReviewID
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Create new review failed"
                });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewDTO reviewDTO)
        {
            try
            {
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
                    throw new Exception("Update review failed");
                }
                await hub.Clients.Group(reviewDTO.MovieID.ToString()).SendAsync("UpdateReview", review);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update review success"
                });
            }
            catch
            {
                return Ok(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Update review failed"
                });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteReview([FromBody] Guid ReviewID)
        {
            try
            {
                var review = context.Reviews.SingleOrDefault(r => r.ReviewID == ReviewID);
                if (review == null)
                {
                    return BadRequest(new ApiResponse
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
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete review success"
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Delete review failed"
                });
            }
        }
    }
}
