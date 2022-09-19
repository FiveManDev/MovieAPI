using Microsoft.AspNetCore.Authorization;
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
        public UserController(MovieAPIDbContext movieAPIDbContext, ILogger<UserController> iLogger)
        {
            context = movieAPIDbContext;
            logger = iLogger;
        }
        [HttpPost]
        public IActionResult CreateUser(string UserName, string Password, string Email)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.StartMethod());
            Guid userId = new Guid();
            try
            {
                if (context.Authorizations != null)
                {
                    int minAuthorizationLevel = context.Authorizations.Min(auth => auth.AuthorizationLevel);
                    Guid auth = context.Authorizations.Where(s => s.AuthorizationLevel == minAuthorizationLevel).First().AuthorizationID;
                    var user = new User
                    {
                        UserName = UserName,
                        Password = Password,
                        AuthorizationID = auth
                    };
                   
                    context.Users!.Add(user);
                    int returnValue =  context.SaveChanges();
                    userId = user.UserID;
                    int minClassLevel = context.Classifications!.Min(auth => auth.ClassLevel);
                    Guid classID = context.Classifications!.Where(s => s.ClassLevel == minClassLevel).First().ClassID;
                    var profile = new Profile
                    {
                        EMail = Email,
                        UserID = userId,
                        ClassID = classID
                    };
                    context.Profiles!.Add(profile);
                    context.SaveChanges();
                    if (returnValue == 0)
                    {
                        throw new Exception("Create new data failed");
                    }
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess());
                }
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.EndMethod());
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create Account Success",
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError(ex.ToString()));
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.EndMethod());
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Create new user failed",
                });
            }


        }
        [HttpPost]
        public IActionResult Login(string UserName,string Password)
        {
            logger.LogError(MethodBase.GetCurrentMethod()!.Name.StartMethod());
            try
            {
                var user = context.Users!.FirstOrDefault(user => user.UserName == UserName 
                                                             && user.Password == Password);
                if(user != null)
                {
                    var TokenManager = new TokenManager(context, logger);
                    Console.WriteLine();
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Login Success",
                        Data = TokenManager.GenerateAccessToken(user)
                    }); 
                }
                else
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Account Not Found"
                    });
                }
                
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError(ex.ToString()));
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.EndMethod());
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Create new user failed"
                });
            }
        }
    }
}
