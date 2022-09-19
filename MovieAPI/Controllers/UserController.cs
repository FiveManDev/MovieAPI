using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
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
        public async Task<IActionResult> CreateUser(string UserName, string Password, string Email)
        {
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
                    int returnValue = await context.SaveChangesAsync();
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
                    if(await context.SaveChangesAsync() == 0)
                    {
                        throw new Exception("Create new data failed");
                    }
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("Profile"));
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create Account Success",
                });
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
        public async Task<IActionResult> Login(string UserName,string Password)
        {
            try
            {
                var user = await context.Users!.FirstOrDefaultAsync(user => user.UserName == UserName 
                                                             && user.Password == Password);
                if (user != null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
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
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User",ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Create new user failed"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string AccessToken,string RefreshToken)
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
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
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

                //check 4: Check refreshtoken exist in DB
                var storedToken = context.Tokens!.FirstOrDefault(x => x.RefreshToken == tokenModel.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token does not exist"
                    });
                }

                //check 5: check refreshToken is used/revoked?
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

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
                if (storedToken.TokenID.ToString() != jti)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Token doesn't match"
                    });
                }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                context.Update(storedToken);
                await context.SaveChangesAsync();
                var TokenManager = new TokenManager(context, logger);
                //create new token
                var user = await context.Users!.SingleOrDefaultAsync(user => user.UserID == storedToken.UserID);
                var token = TokenManager.GenerateAccessToken(user);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Renew token success",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Something went wrong"
                });
            }
        }
    }
}
