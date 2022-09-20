using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Services;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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
        public ActionResult CreateUser(string UserName, string Password, string Email)
        {
            Guid userId = new Guid();
            try
            {
                var userCheck = context.Users!.FirstOrDefault(user => user.UserName == UserName);
                if (userCheck == null)
                {
                    int minAuthorizationLevel = context.Authorizations!.Min(auth => auth.AuthorizationLevel);
                    Guid auth = context.Authorizations!.Where(s => s.AuthorizationLevel == minAuthorizationLevel).First().AuthorizationID;
                    var user = new User
                    {
                        UserName = UserName,
                        Password = Password,
                        AuthorizationID = auth
                    };
                    context.Users!.Add(user);
                    int returnValue = context.SaveChanges();
                    if (returnValue == 0)
                    {
                        throw new Exception("Create new data failed");
                    }
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("User"));
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
                    var checkProfileSave = context.SaveChanges();
                    if (checkProfileSave == 0)
                    {
                        throw new Exception("Create new data failed");
                    }
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("Profile"));

                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Create Account Success",
                    });
                }
                else
                {
                    return Conflict(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Account already exists"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("User", ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Create new user failed",
                });
            }
        }
        [HttpPost]
        public IActionResult Login(string UserName, string Password)
        {
            try
            {
                var user = context.Users!.FirstOrDefault(user => user.UserName == UserName
                                                             && user.Password == Password);
                if (user != null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
                    var TokensExist = context.Tokens!.FirstOrDefault(token => token.UserID == user.UserID);
                    if(TokensExist!= null)
                    {
                        context.Tokens!.Remove(TokensExist);
                        context.SaveChanges();
                    }
                    var TokenManager = new TokenManager(context, logger);
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
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Login failed"
                });
            }
        }
        [HttpPost]
        public IActionResult RefreshToken(string AccessToken, string RefreshToken)
        {
            TokenModel tokenModel = new TokenModel
            {
                AccessToken = AccessToken,
                RefreshToken = RefreshToken
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(AppSettings.SecretKey!);
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            try
            {
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, tokenValidateParam, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return Ok(new ApiResponse
                        {
                            IsSuccess = false,
                            Message = "Invalid Token"
                        });
                    }
                }
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);
                var expireDate = MyConvert.ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Access token has not yet expired"
                    });
                }
                var storedToken = context.Tokens!.FirstOrDefault(x => x.RefreshToken == tokenModel.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token does not exist"
                    });
                }
                if (storedToken.IsUsed)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token has been used"
                    });
                }
                if (storedToken.IsRevoked)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token has been revoked"
                    });
                }
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                context.Tokens!.Update(storedToken);
                context.SaveChanges();
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PutDataSuccess("Token", 1));
                var TokenManager = new TokenManager(context, logger);
                var user = context.Users!.SingleOrDefault(user => user.UserID == storedToken.UserID);
                var token = TokenManager.GenerateAccessToken(user!);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Renew token success",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataError("Token", ex.ToString()));
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Something went wrong"
                });
            }
        }
        [HttpGet]
        [Authorize]
        public IActionResult DecodeToken()
        {
            TokenManager tokenManager = new TokenManager(context, logger);
            var token = Request.Headers["Authorization"];
            string userRole = tokenManager.DecodeToken(token).AuthorizationID!;
            return Ok(new ApiResponse
            {
                IsSuccess=true,
                Message="",
                Data = userRole
            });
        }
    }
}
