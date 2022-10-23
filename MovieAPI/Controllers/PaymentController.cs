using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using System.Reflection;
using System.Web;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    //[Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly ILogger<PaymentController> logger;
        public PaymentController(MovieAPIDbContext context, ILogger<PaymentController> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult GetTotalMoney()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var maxClassLevel = context.Classifications
                    .OrderBy(context => context.ClassLevel)
                    .First();
                var totalProfile = context.Profiles
                    .Where(context => context.ClassID == maxClassLevel.ClassID)
                    .Count();
                if (totalProfile == 0)
                {
                    logger.LogError(MethodBase.GetCurrentMethod().Name.GetDataError("Profile", "No users of Premium tier yet"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "No users of Premium tier yet"
                    });
                }
                var totalMoney = totalProfile * maxClassLevel.ClassPrice;
                logger.LogError(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Profile", totalProfile));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get total money success",
                    Data = totalMoney
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Classification", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
       

    }
}
