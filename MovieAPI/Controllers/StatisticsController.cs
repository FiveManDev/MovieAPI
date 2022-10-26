using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using System.Globalization;
using System.Reflection;

namespace MovieAPI.Controllers;

[Route("api/v{version:apiVersion}/[controller]/[Action]")]
[ApiController]
[ApiVersion("1")]
public class StatisticsController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly MovieAPIDbContext _db;

    public StatisticsController(ILogger<UserController> iLogger, MovieAPIDbContext db)
    {
        logger = iLogger;
        _db = db;
    }

    [HttpGet]
    public IActionResult GetStatisticsForMonth(string date)
    {
        try
        {
            date = date != null ? date.Trim() : DateTime.Now.ToString("MMyyyy");

            DateTime datetime = DateTime.ParseExact(date, "MMyyyy", CultureInfo.InvariantCulture);
            int userCreatedCount = _db.Users.Where(user => user.CreateAt.Month == datetime.Month 
                                                && user.CreateAt.Year == datetime.Year).Count();
            int movieCreatedCount = _db.MovieInformations.Where(movie => movie.PublicationTime.Month == datetime.Month
                                                             && movie.PublicationTime.Year == datetime.Year).Count();
            int reviewCreatedCount = _db.Reviews.Where(review => review.ReviewTime.Month == datetime.Month
                                                    && review.ReviewTime.Year == datetime.Year).Count();
            double totalMoneyGained = userCreatedCount * 10 + movieCreatedCount * 5 + reviewCreatedCount * 2;

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Data = new {
                    userCreatedCount,
                    movieCreatedCount,
                    reviewCreatedCount,
                    totalMoneyGained
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("None", ex.ToString()));
            return StatusCode(500, new ApiResponse
            {
                IsSuccess = false,
                Message = "Internal Server Error"
            });
        }
    }

}