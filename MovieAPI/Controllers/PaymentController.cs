using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Services.MomoPayment;
using Newtonsoft.Json.Linq;
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
        [HttpGet]
        public async Task<IActionResult> PaymentRequest([FromQuery] PaymentModel paymentModel)
        {
            try
            {
                if (paymentModel == null)
                {
                    throw new Exception("Data is null");
                }
                var paymentUrl = await MomoConnection.MomoResponse(paymentModel);
                if (paymentUrl == null)
                {
                    throw new Exception("Get payment url failed");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get payment url success",
                    Data = paymentUrl
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Get payment url failed"
                });
            }
        }
        [HttpGet]
        public async void GetInfor()
        {
            HttpContext.Request.ContentType = "application/json";
            var reader = new StreamReader(HttpContext.Request.Body);
            var body = await reader.ReadToEndAsync();
            var collection = HttpUtility.ParseQueryString(body);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(collection.AllKeys.ToDictionary(y => y, y => collection[y]));
            var data = JObject.Parse(json);
            int errorCode = Int32.Parse(data.GetValue("errorCode").ToString());
            int userID = Int32.Parse(data.GetValue("extraData").ToString());
            if (errorCode == 0)
            {

                Console.WriteLine(userID);
            }

        }

    }
}
