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
using MovieAPI.Models.Pagination;
using MovieAPI.Services;
using MovieAPI.Services.Attributes;
using MovieAPI.Services.Mail;
using MovieWebApp.Models;
using MovieWebApp.Utility.Extension;
using Newtonsoft.Json.Linq;
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

        public UserController(ILogger<UserController> iLogger, MovieAPIDbContext db, IMapper mapper)
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
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var userCheck = _db.Users!.FirstOrDefault(user => user.UserName == createUserDTO.UserName);
                if (userCheck != null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.PostDataError("User", "Account already exists"));
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
                    CreateAt = DateTime.Now
                };
                _db.Users!.Add(user);
                int returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of User failed");
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
                    throw new Exception("Save data of profile failed");
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
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("User - Profile", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpPost]
        public ActionResult Login([FromBody] LoginDTO loginUserDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = _db.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .FirstOrDefault(user => user.UserName == loginUserDTO.UserName);
                if (user == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User not found"));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Login Failed! Incorrect username or password!",
                    });
                }
                if (HashPassword.VerifyPasswordHash(loginUserDTO.Password, user.PasswordHash, user.PasswordSalt))
                {
                    bool isBanned = !user.Status;
                    if (isBanned)
                    {
                        return StatusCode(403, new ApiResponse
                        {
                            IsSuccess = false,
                            Message = "Login Failed! The account is banned!"
                        });
                    }

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
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User erorr"));
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
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [Authorize]
        [UserBanned]
        [HttpPost]
        public ActionResult ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                if (!changePasswordDTO.NewPassword.Equals(changePasswordDTO.ConfirmPassword))
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "New password and confirm password are not the same!"));
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "New password and confirm password are not the same!",
                    });
                }
                string userId = User.Claims.FirstOrDefault(claim => claim.Type == "UserID").Value;
                if (userId == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Not found account!",
                    });
                }
                var user = _db.Users.FirstOrDefault(user => user.UserID.ToString().Equals(userId));
                if (user == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User not found"));
                    return NotFound(new ApiResponse
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
                        throw new Exception("Save data of User failed");
                    }
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", 1));
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Message = "Change password success",
                    });
                }
                else
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", "Incorrect password"));
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Incorrect password!",
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [UserBanned]
        [HttpPost]
        public IActionResult RefreshToken(string AccessToken, string RefreshToken)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
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
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpPost]
        public IActionResult LoginWithService(ServiceLoginModel serviceLoginModel)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var existProfile = _db.Profiles.SingleOrDefault(pro => pro.Email == serviceLoginModel.Mail.ToLower());
                if (existProfile != null)
                {
                    var existuser = _db.Users.SingleOrDefault(user => user.UserID == existProfile.UserID);
                    var TokenExít = TokenManager.GenerateAccessToken(existuser);
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("User", 1));
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
                    UserName = serviceLoginModel.Id + "Google",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    AuthorizationID = auth ,
                    Status = true,
                    CreateAt = DateTime.Now
                };
                _db.Users!.Add(user);
                int returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of user failed");
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
                    throw new Exception("Save data of profile failed");
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
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpPost]
        public IActionResult ConfirmEmail([FromBody] string email)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var code = RandomText.RandomByNumberOfCharacters(6, EnumObject.RandomType.Number);
                var mailName = email.Substring(0, email.IndexOf("@"));
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
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("No table", 0));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send code to mail success",
                    Data = code
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpPost]
        public IActionResult ConfirmEmailForgotPassword([FromBody] string email)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
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
                    $"Thank you," +
                    $" <br/>" +
                    $"{AppSettings.MailTile} account group"

                };
                MailService.SendMail(mailModel);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("No table", 0));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send code to mail success",
                    Data = code
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpPost]
        public ActionResult ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var profile = _db.Profiles.FirstOrDefault(pro => pro.Email == resetPasswordDTO.Email);
                if (profile == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("Profile", "Account not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Account not found"
                    });
                }
                if (!string.Equals(resetPasswordDTO.NewPassword, resetPasswordDTO.ConfirmPassword))
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("Profile", "Password and Confirm Password are not the sam"));
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
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Profile", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Reset password success",
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult DeleteUser(Guid UserId)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = _db.Users.Find(UserId);
                var reviews = _db.Reviews.Where(r=>r.UserID== UserId).ToList();
                if (reviews.Count != 0)
                {
                    _db.Reviews.RemoveRange(reviews);
                    var returnValueMovie = _db.SaveChanges();
                    if (returnValueMovie == 0)
                    {
                        throw new Exception("Delete data of Movie failed");
                    }
                }
                if (user == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User not found"));
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
                    throw new Exception("Delete data of user failed");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.DeleteDataSuccess("User", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Delete user success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult ChangeUserStatus([FromBody] JObject data)
        {
            try
            {
                Guid UserID = data["userID"].ToObject<Guid>();
                bool IsBanned = data["isBanned"].ToObject<bool>();
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = _db.Users.Find(UserID);
                if (user == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
                user.Status = IsBanned;
                _db.Users.Update(user);
                var returnValue = _db.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Update data of user failed");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Change user status success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetClassOfUser(Guid userID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var profile = _db.Profiles.FirstOrDefault(pro => pro.UserID == userID);
                if (profile == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
                var classification = _db.Classifications.Find(profile.ClassID);
                if (classification == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("Classification", "Classification not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Class not found"
                    });
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Classification", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get class of user",
                    Data = classification.ClassName
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetLatestCreatedAccount(int top)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = _db.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .OrderByDescending(user => user.CreateAt)
                    .Take(top).ToList();
                if (user.Count==0)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("User", "User is empty"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User is empty"
                    });
                }
                var userInformation = mapper.Map<List<User>, List<UserDTO>>(user);
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.GetDataSuccess("User", user.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get the latest created account success",
                    Data = userInformation
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("User", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet]
        public IActionResult GetUsers([FromQuery] Pager pager, string q, string sortBy, string sortType)
        {
            try
            {
                q = (q == null) ? "" : q.Trim().ToLower();
               
                List<User> users = _db.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Profile.Classification)
                    .Include(user => user.Reviews)
                    .Where(user => user.UserName.ToLower().Contains(q)
                        || user.Profile.FirstName.ToLower().Contains(q)
                        || user.Profile.LastName.ToLower().Contains(q)).ToList();

                if (users.Count == 0)
                {
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Data = null
                    });
                }

                // can not map PaginatedList???
                var userDTOs = mapper.Map<List<User>, List<UserDTO>>(users);

                sortBy = (sortBy == null) ? "date" : sortBy.Trim().ToLower();
                sortType = (sortType == null) ? "desc" : sortType.Trim().ToLower();
                if (sortBy == "date")
                {
                    if (sortType == "desc")
                    {
                        userDTOs = userDTOs.OrderByDescending(user => user.CreateAt).ToList();
                    }
                    else if (sortType == "asc")
                    {
                        userDTOs = userDTOs.OrderBy(user => user.CreateAt).ToList();
                    }
                }
                else if (sortBy == "status")
                {
                    if (sortType == "desc")
                    {
                        userDTOs = userDTOs.OrderByDescending(user => user.Status).ToList();
                    }
                    else if (sortType == "asc")
                    {
                        userDTOs = userDTOs.OrderBy(user => user.Status).ToList();
                    }
                }

                PaginatedList<UserDTO> result = PaginatedList<UserDTO>.ToPageList(userDTOs, pager.pageIndex, pager.pageSize);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Data = new
                    {
                        users = result,
                        pager = result.paginationDTO
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("None", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
