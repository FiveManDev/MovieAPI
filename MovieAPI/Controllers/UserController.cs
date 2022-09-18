using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Services;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class UserController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly ILogger<UserController> logger;
        public UserController(MovieAPIDbContext movieAPIDbContext,ILogger<UserController> iLogger)
        {
            context = movieAPIDbContext;
            logger = iLogger;
        }
        [HttpGet]
        public IActionResult GetUser()
        {
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.StartMethod());
            var users = new List<User>();
            if (context.Users != null)
            {
                users= context.Users.ToList();
                foreach (var item in users)
                {
                    Console.WriteLine(item);
                }
            }
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.EndMethod());
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Message = "Success",
                Data = ""

            });

        }
    }
}
