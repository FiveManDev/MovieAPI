using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;
using MovieAPI.Services.MomoPayment;
using Newtonsoft.Json.Linq;
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

        public PaymentController(MovieAPIDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult GetTotalMoney()
        {
            try
            {
                var maxClassLevel = context.Classifications
                    .OrderBy(context => context.ClassLevel)
                    .First();
                var totalProfile = context.Profiles
                    .Where(context=>context.ClassID==maxClassLevel.ClassID)
                    .Count();
                if(totalProfile == 0)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "No users of Premium tier yet"
                    });
                }
                var totalMoney = totalProfile* maxClassLevel.ClassPrice;
                return Ok(new ApiResponse
                {
                    IsSuccess =true,
                    Message = "Get total money success",
                    Data = totalMoney
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
        public async Task<IActionResult> PaymentRequest([FromQuery] PaymentModel paymentModel)
        {
            try
            {
                if (paymentModel == null)
                {
                    throw new Exception("Data is null");
                }
                var paymentUrl = await MomoConnection.MomoResponse(paymentModel);
                if(paymentUrl == null)
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
