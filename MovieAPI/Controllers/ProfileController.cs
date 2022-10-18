using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class ProfileController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly MovieAPIDbContext context;
        private readonly ILogger<ProfileController> logger;
        public ProfileController(IMapper mapper, MovieAPIDbContext context, ILogger<ProfileController> logger)
        {
            _mapper = mapper;
            this.context = context;
            this.logger = logger;
        }
        //[Authorize]
        [HttpGet]
        public IActionResult GetInformation([Required] Guid UserID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .FirstOrDefault(user => user.UserID == UserID);
                if (user != null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Users - Profile - Authorization - Classification", 1));
                    var userDTO = _mapper.Map<User, UserDTO>(user);
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
                        Data = userDTO
                    });
                }
                else
                {
                    logger.LogError(MethodBase.GetCurrentMethod().Name.GetDataError("Users - Profile - Authorization - Classification", "User not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Users - Profile - Authorization - Classification", ex.ToString()));
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Cannot Get User Information! Something wrong!"
                });
            }
        }
        //[Authorize(Roles ="Admin")]
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    try
        //    {

        //    catch
        //    {

        //    }
        //}
        [HttpGet]
        public IActionResult GetInformationSortByCreateTime()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .OrderBy(context => context.CreateAt)
                    .ToList();
                var UserDTO = _mapper.Map<List<User>, List<UserDTO>>(user);
                foreach (var item in UserDTO)
                {
                    item.NumberOfReviews = getNumberOfReviews(item.UserID);
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Users - Profile - Authorization - Classification", user.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = UserDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Users - Profile - Authorization - Classification", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult SearchUser(string text)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var users = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .Where(user => user.Profile.FirstName.Contains(text)
                    || user.Profile.LastName.Contains(text)
                    || user.Profile.LastName.Contains(text))
                    .ToList();
                var userDTO = _mapper.Map<List<User>, List<UserDTO>>(users);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Users - Profile - Authorization - Classification", users.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = userDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Users - Profile - Authorization - Classification", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetInformationSortByStatus()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = context.Users
                   .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .OrderByDescending(context => context.Status)
                    .ToList();
                var UserDTO = _mapper.Map<List<User>, List<UserDTO>>(user);
                foreach (var item in UserDTO)
                {
                    item.NumberOfReviews = getNumberOfReviews(item.UserID);
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Users - Profile - Authorization - Classification", user.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by status",
                    Data = UserDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Users - Profile - Authorization - Classification", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [HttpGet]
        public IActionResult GetInformationSortByClass()
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var user = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .OrderByDescending(context => context.Profile.Classification.ClassLevel)
                    .ToList();
                var UserDTO = _mapper.Map<List<User>, List<UserDTO>>(user);
                foreach (var item in UserDTO)
                {
                    item.NumberOfReviews = getNumberOfReviews(item.UserID);
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Users - Profile - Authorization - Classification", user.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by class",
                    Data = UserDTO
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Users - Profile - Authorization - Classification", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [HttpPut]
        public IActionResult PremiumUpgrade([FromBody] Guid userID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var profile = context.Profiles.SingleOrDefault(pro => pro.UserID == userID);
                if (profile == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataError("Profile", "Profile not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Profile not found"
                    });
                }
                var maxLevel = context.Classifications.Max(cl => cl.ClassLevel);
                var classification = context.Classifications.SingleOrDefault(cl => cl.ClassLevel == maxLevel);
                profile.ClassID = classification.ClassID;
                context.Profiles.Update(profile);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of profile failed");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PutDataSuccess("Profile", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Premium Upgrade success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PutDataError("Profile", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [HttpPut]
        public IActionResult UpdateProfileForUser([FromBody] JObject data)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                Guid UserID = data["userID"].ToObject<Guid>();
                string FirstName = data["firstName"].ToObject<string>();
                string LastName = data["lastName"].ToObject<string>();
                var profile = context.Profiles.SingleOrDefault(pro => pro.UserID == UserID);
                if (profile == null)
                {
                    logger.LogInformation(MethodBase.GetCurrentMethod().Name.PutDataError("Profile", "Profile not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Profile not found"
                    });
                }
                profile.FirstName = FirstName;
                profile.LastName = LastName;
                context.Profiles.Update(profile);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of profile failed");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PutDataSuccess("Profile", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update profile success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PutDataError("Profile", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult UpdateProfileForAdmin([FromBody] JObject data)
        {

            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                Guid UserID = data["userID"].ToObject<Guid>();
                string FirstName = data["firstName"].ToObject<string>();
                string LastName = data["lastName"].ToObject<string>();
                Guid ClassID = data["classID"].ToObject<Guid>();
                Guid AuthorizationID = data["authorizationID"].ToObject<Guid>();
                var user = context.Users
                    .Include(user => user.Profile)
                    .SingleOrDefault(user => user.UserID == UserID);
                if (user == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod().Name.PutDataError("Profile", "Profile Not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Profile Not found"
                    });
                }
                var profile = user.Profile;
                profile.FirstName = FirstName;
                profile.LastName = LastName;
                profile.ClassID = ClassID;
                user.AuthorizationID = AuthorizationID;
                context.Update(user);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Save data of profile failed");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update profile success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PutDataError("Profile", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public int getNumberOfReviews(Guid UserID)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            var count = context.Reviews.Where(context => context.UserID == UserID).Count();

            return count;
        }
    }
}
