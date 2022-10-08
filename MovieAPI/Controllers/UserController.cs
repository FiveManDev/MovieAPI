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
using MovieAPI.Services.Mail;
using MovieWebApp.Models;
using MovieWebApp.Utility.Extension;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IMapper _mapper;
        private readonly MovieAPIDbContext _db;

        public UserController(ILogger<UserController> iLogger, IMapper mapper, MovieAPIDbContext db)
        {
            logger = iLogger;
            _mapper = mapper;
            _db = db;
        }

        // Get user information
        [Authorize]
        [HttpGet]
        [ActionName("user-information")]
        public IActionResult GetUserInformation([Required]string id)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == "UserID").Value;
                if (!userId.Equals(id))
                {
                    return StatusCode(401, new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "You are not allowed to get user information"
                    });
                }

                var user = _db.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .FirstOrDefault(user => user.UserID.ToString() == id);

                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Cannot Get User Information!"
                    });
                }
                var userDTO = _mapper.Map<User, UserDTO>(user);

                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = userDTO
                });
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
        public ActionResult CreateUser([FromBody] CreateUserRequestDTO createUserDTO)
        {
            try
            {
                var userCheck = _db.Users!.FirstOrDefault(user => user.UserName == createUserDTO.UserName);
                if (userCheck != null)
                {
                    return Conflict(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Account already exists"
                    });
                }

                int minAuthorizationLevel = _db.Authorizations!.Min(auth => auth.AuthorizationLevel);
                Guid auth = _db.Authorizations!.FirstOrDefault(s => s.AuthorizationLevel == minAuthorizationLevel).AuthorizationID;
                
                HashPassword.CreatePasswordHash(createUserDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user = new User
                {
                    UserName = createUserDTO.UserName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    AuthorizationID = auth
                };
                _db.Users!.Add(user);
                int returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Create new data failed");
                }
                
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("User"));
                Guid userId = user.UserID;
                int minClassLevel = _db.Classifications!.Min(auth => auth.ClassLevel);
                Guid classID = _db.Classifications!.FirstOrDefault(s => s.ClassLevel == minClassLevel).ClassID;
                
                var profile = new Data.Profile
                {
                    Email = createUserDTO.Email,
                    UserID = userId,
                    ClassID = classID
                };
                
                _db.Profiles!.Add(profile);
                var checkProfileSave = _db.SaveChanges();
                if (checkProfileSave == 0)
                {
                    throw new Exception("Create new data failed");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("Profile"));

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create Account Success"
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
        public ActionResult Login([FromBody] LoginDTO loginUserDTO)
        {
            try
            {
                var user = _db.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .FirstOrDefault(user => user.UserName == loginUserDTO.UserName);

                if (user == null)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Login Failed! Incorrect username or password!",
                    });
                }

                if (HashPassword.VerifyPasswordHash(loginUserDTO.Password, user.PasswordHash, user.PasswordSalt))
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
                    var TokensExist = _db.Tokens!.FirstOrDefault(token => token.UserID == user.UserID);
                    if (TokensExist != null)
                    {
                        _db.Tokens!.Remove(TokensExist);
                        _db.SaveChanges();
                    }
                    var Token = TokenManager.GenerateAccessToken(user);
                    var tokenData = new Token
                    {
                        AccessToken = Token.AccessToken,
                        RefreshToken = Token.RefreshToken,
                        UserID = user.UserID
                    };
                    _db.Tokens?.Add(tokenData);
                    _db.SaveChanges();
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
                        IsSuccess = false,
                        Message = "Login Failed! Incorrect username or password!"
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

        // Change password
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try {

                if (!changePasswordDTO.NewPassword.Equals(changePasswordDTO.ConfirmPassword))
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "New password and confirm password are not the same!",
                    });
                }

                string userId = User.Claims.FirstOrDefault(claim => claim.Type == "UserID").Value;
                
                if (userId == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Not found account!",
                    }); 
                } 

                var user = _db.Users.FirstOrDefault(user => user.UserID.ToString().Equals(userId));

                if (user == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Not found account!",
                    });
                }

                if (HashPassword.VerifyPasswordHash(changePasswordDTO.OldPassword, user.PasswordHash, user.PasswordSalt))
                {
                    HashPassword.CreatePasswordHash(changePasswordDTO.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

                    user.PasswordSalt = passwordSalt;
                    user.PasswordHash = passwordHash;
                    
                    _db.Users.Update(user);
                    int result = _db.SaveChanges();

                    if (result == 0)
                    {
                        return StatusCode(500, new ApiResponse
                        {
                            IsSuccess = false,
                            Message = "Can't change password, something wrong",
                        });
                    }

                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Change password success",
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Incorrect password!",
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
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
                var storedToken = _db.Tokens!.FirstOrDefault(token => token.RefreshToken == tokenModel.RefreshToken
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
                _db.Tokens!.Update(storedToken);
                _db.SaveChanges();
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

        [HttpPost]
        public IActionResult LoginWithService(ServiceLoginModel serviceLoginModel)
        {
            try
            {
                var existProfile = _db.Profiles.SingleOrDefault(pro => pro.Email == serviceLoginModel.Mail.ToLower());
                if (existProfile != null)
                {
                    var existuser = _db.Users.SingleOrDefault(user => user.UserID == existProfile.UserID);
                    var Token = TokenManager.GenerateAccessToken(existuser);
                    return Ok(Token);
                }
                int minAuthorizationLevel = _db.Authorizations!.Min(auth => auth.AuthorizationLevel);
                Guid auth = _db.Authorizations!.FirstOrDefault(s => s.AuthorizationLevel == minAuthorizationLevel).AuthorizationID;
                HashPassword.CreatePasswordHash(serviceLoginModel.Id, out byte[] passwordHash, out byte[] passwordSalt);
                var user = new User
                {
                    UserName = serviceLoginModel.Id+"Google",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    AuthorizationID = auth
                };
                _db.Users!.Add(user);
                int returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Create new data failed");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("User"));
                Guid userId = user.UserID;
                int minClassLevel = _db.Classifications!.Min(auth => auth.ClassLevel);
                Guid classID = _db.Classifications!.FirstOrDefault(s => s.ClassLevel == minClassLevel).ClassID;
                var profile = new Data.Profile
                {
                    Email = serviceLoginModel.Mail.ToLower(),
                    UserID = userId,
                    ClassID = classID,
                    FirstName = serviceLoginModel.FirstName,
                    LastName = serviceLoginModel.LastName
                };
                _db.Profiles!.Add(profile);
                var checkProfileSave = _db.SaveChanges();
                if (checkProfileSave == 0)
                {
                    throw new Exception("Create new data failed");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("Profile"));

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create Account Success"
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Login failed"
                });
            }
        }
        [HttpPost]
        public IActionResult ConfirmEmail([FromBody] string email)
        {
            try
            {
                var code = RandomText.RandomByNumberOfCharacters(6,EnumObject.RandomType.Number);
                var mailName = email.Substring(0,email.IndexOf("@"));
                var mailModel = new MailModel
                {
                    EmailTo = email,
                    Subject = "Confirm your email address",
                    Body = $"Welcome {mailName.ToLower()}!" +
                    $"<br/><br/>" +
                    $"Thanks for signing up with {AppSettings.MailTile}!" +
                    $"<br/><b>{code}</b> is your {AppSettings.MailTile} verification." +
                    $" <br/>" +
                    $"Have fun coding, and don't hesitate to contact us with your feedback."
                };
                MailService.SendMail(mailModel);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send code to mail success",
                    Data = code
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Send code to maill failed"
                });
            }
        }
    }
}
