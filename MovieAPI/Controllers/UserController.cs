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
        private readonly MovieAPIDbContext _db;
        private readonly IMapper mapper;

        public UserController(ILogger<UserController> iLogger, MovieAPIDbContext db,IMapper mapper)
        {
            logger = iLogger;
            _db = db;
            this.mapper = mapper;
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
                    AuthorizationID = auth,
                    Status = true,
                    CreateAt= DateTime.Now
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
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Login failed"
                });
            }
        }
        [Authorize]
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
                    var TokenExít = TokenManager.GenerateAccessToken(existuser);
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Login Success",
                        Data = TokenExít
                    });
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
                var Token = TokenManager.GenerateAccessToken(user);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Create Account and login Success",
                    Data = Token
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
                    $"Thank you," +
                    $" <br/>" +
                    $"{AppSettings.MailTile} account group"
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
        [HttpPost]
        public IActionResult ConfirmEmailForgotPassword([FromBody] string email)
        {
            try
            {
                var code = RandomText.RandomByNumberOfCharacters(6, EnumObject.RandomType.Number);
                var mailName = email.Substring(0, email.IndexOf("@"));
                var mailModel = new MailModel
                {
                    EmailTo = email,
                    Subject = $"Reset {AppSettings.MailTile} account password",
                    Body = $"Hello {mailName.ToLower()}!" +
                    $"<br/><br/>" +
                    $"Please use this code to reset the password for your {AppSettings.MailTile} account {email}" +
                    $"<br/>Here is your code: <b>{code}</b>." +
                    $" <br/>" +
                    $"Thank you,"+
                    $" <br/>" +
                    $"{AppSettings.MailTile} account group"

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
        [HttpPost]
        public ActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var profile = _db.Profiles.FirstOrDefault(pro => pro.Email == resetPasswordDTO.Email);
                if (profile == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Account not found"
                    });
                }
                if (!string.Equals(resetPasswordDTO.NewPassword, resetPasswordDTO.ConfirmPassword))
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Password and Confirm Password are not the same"
                    });
                }
                var user = _db.Users.FirstOrDefault(user => user.UserID == profile.UserID);
                HashPassword.CreatePasswordHash(resetPasswordDTO.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;
                _db.Update(user);
                _db.SaveChanges();
                return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Reset password success",
                    });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Reset password failed"
                });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete]
        public IActionResult DeleteUser([FromBody] Guid UserId)
        {
            try
            {
                var user = _db.Users.Find(UserId);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
                _db.Users.Remove(user);
                var returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete user success"
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal server error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult ChangeUserStatus([FromBody] (Guid UserID, bool IsBanned) parameters)
        {
            try
            {
                var user = _db.Users.Find(parameters.UserID);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
                user.Status = parameters.IsBanned;
                _db.Users.Update(user);
                var returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Change user status success"
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = ""
                });
            }
        }
        [HttpGet]
        public IActionResult GetClassOfUser(Guid userID)
        {
            try
            {
                var profile = _db.Profiles.FirstOrDefault(pro=>pro.UserID== userID);
                if(profile == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
                var classification = _db.Classifications.Find(profile.ClassID);
                if (classification == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Class not found"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get class of user",
                    Data = classification.ClassName
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = ""
                });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetLatestCreatedAccount(int top)
        {
            try
            {
                var user = _db.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .OrderByDescending(user=>user.CreateAt)
                    .Take(top).ToList();
                if(user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User is empty"
                    });
                }
                var userInformation =  mapper.Map<List<User>,List<UserDTO>>(user);
                return Ok(new ApiResponse
                {
                    IsSuccess =true,
                    Message = "Get the latest created account success",
                    Data = userInformation
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = ""
                });
            }
        }
    }
}
