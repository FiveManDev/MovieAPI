using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> iLogger, IMapper mapper)
        {
            logger = iLogger;
            _mapper = mapper;
        }

        // Get user information
        [HttpGet("id", Name="GetUserInformation")]
        [Authorize]
        public IActionResult GetUserInformation(string id)
        {
            try
            {
                string useId = User.Claims.FirstOrDefault(claim => claim.Type == "UserID").Value;
                using var context = new MovieAPIDbContext();
                var user = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .FirstOrDefault(user => user.UserID.ToString() == useId);

                if (user != null)
                {

                    var userDTO = _mapper.Map<User, UserDTO>(user);
                   
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Get All Genre Of Movie Success",
                        Data = userDTO
                    });
                }
                else
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Cannot Get User Information!"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("User", ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Cannot Get User Information! Something wrong!"
                });
            }
        }

        [HttpPost]
        public ActionResult CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                using var context = new MovieAPIDbContext();
                var userCheck = context.Users!.FirstOrDefault(user => user.UserName == createUserDTO. UserName);
                if (userCheck == null)
                {
                    int minAuthorizationLevel = context.Authorizations!.Min(auth => auth.AuthorizationLevel);
                    Guid auth = context.Authorizations!.FirstOrDefault(s => s.AuthorizationLevel == minAuthorizationLevel).AuthorizationID;
                    var user = new User
                    {
                        UserName = createUserDTO.UserName,
                        Password = createUserDTO.Password,
                        AuthorizationID = auth
                    };
                    context.Users!.Add(user);
                    int returnValue = context.SaveChanges();
                    if (returnValue == 0)
                    {
                        throw new Exception("Create new data failed");
                    }
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("User"));
                    Guid userId = user.UserID;
                    int minClassLevel = context.Classifications!.Min(auth => auth.ClassLevel);
                    Guid classID = context.Classifications!.FirstOrDefault(s => s.ClassLevel == minClassLevel).ClassID;
                    var profile = new Data.Profile
                    {
                        Email = createUserDTO.Email,
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

                    return CreatedAtRoute(
                       "GetUserInformation",
                       new { id = user.UserID.ToString() },
                       new ApiResponse
                       {
                           IsSuccess = true,
                           Message = "Create Account Success",
                           Data = _mapper.Map<User, UserDTO>(user)
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
                using var context = new MovieAPIDbContext();
                var user = context.Users!.FirstOrDefault(user => user.UserName == UserName
                                                         && user.Password == Password);
                if (user != null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
                    var TokensExist = context.Tokens!.FirstOrDefault(token => token.UserID == user.UserID);
                    if (TokensExist != null)
                    {
                        context.Tokens!.Remove(TokensExist);
                        context.SaveChanges();
                    }
                    var Token = TokenManager.GenerateAccessToken(user);
                    var tokenData = new Token
                    {
                        AccessToken = Token.AccessToken,
                        RefreshToken = Token.RefreshToken,
                        UserID = user.UserID
                    };
                    context.Tokens?.Add(tokenData);
                    context.SaveChanges();
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("Token"));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Login Success",
                        Data = Token
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
                using var context = new MovieAPIDbContext();
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
                var storedToken = context.Tokens!.FirstOrDefault(token => token.RefreshToken == tokenModel.RefreshToken
                                                                          && token.AccessToken == tokenModel.AccessToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token does not exist"
                    });
                }
                storedToken.RefreshToken = Guid.NewGuid().ToString();
                context.Tokens!.Update(storedToken);
                context.SaveChanges();
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PutDataSuccess("Token", 1));
                
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Refresh token successful",
                    Data = new TokenModel
                    {
                        AccessToken = storedToken.AccessToken,
                        RefreshToken = storedToken.RefreshToken
                    }
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
            var token = Request.Headers["Authorization"];
            string userRole = TokenManager.DecodeToken(token).AuthorizationID!;
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Message = "",
                Data = userRole
            });
        }
    }
}
