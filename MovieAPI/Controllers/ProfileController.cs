using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models.DTO;
using MovieAPI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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

        public ProfileController(IMapper mapper, MovieAPIDbContext context)
        {
            _mapper = mapper;
            this.context = context;
        }
        //[Authorize]
        [HttpGet]
        public IActionResult GetInformation([Required] Guid UserID)
        {
            try
            {
                var user = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user=>user.Profile.Classification)
                    .FirstOrDefault(user => user.UserID == UserID);
                if (user != null)
                {
                    var userDTO = _mapper.Map<User, UserDTO>(user);
                    return Ok(new ApiResponse
                    {
                        IsSuccess = true,
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
            catch 
            {
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
                var user = context.Users
                   .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .OrderBy(context => context.CreateAt)
                    .ToList();
                var UserDTO= _mapper.Map<List<User>,List<UserDTO>>(user);
                foreach(var item in UserDTO)
                {

                    item.NumberOfReviews = getNumberOfReviews(item.UserID);
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = UserDTO
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
        public IActionResult SearchUser(string text)
        {
            try
            {
                var users = context.Users
                    .Include(user => user.Profile)
                    .Include(user => user.Authorization)
                    .Include(user => user.Profile.Classification)
                    .Where(user => user.Profile.FirstName.Contains(text)
                    || user.Profile.LastName.Contains(text)
                    || user.Profile.LastName.Contains(text))
                    .ToList();
                var userDTO = _mapper.Map<List<User>, List<UserDTO>>(users);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by create time",
                    Data = userDTO
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
        public IActionResult GetInformationSortByStatus()
        {
            try
            {
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
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by status",
                    Data = UserDTO
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
        public IActionResult GetInformationSortByClass()
        {
            try
            {
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
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get information sort by class",
                    Data = UserDTO
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
        [Authorize]
        [HttpPut]
        public IActionResult PremiumUpgrade([FromBody] Guid userID)
        {
            try
            {
                var profile = context.Profiles.SingleOrDefault(pro=>pro.UserID==userID);
                if(profile == null)
                {
                    throw new Exception("Profile not found");
                }
                var maxLevel = context.Classifications.Max(cl=>cl.ClassLevel);
                var classification = context.Classifications.SingleOrDefault(cl => cl.ClassLevel == maxLevel);
                if (classification == null)
                {
                    throw new Exception("Premium Upgrade failed");
                }
                profile.ClassID = classification.ClassID;
                context.Profiles.Update(profile);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Premium Upgrade failed");
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Premium Upgrade success"
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Premium Upgrade failed"
                });
            }
        }
        [Authorize]
        [HttpPut]
        public IActionResult UpdateProfileForUser([FromBody] (Guid UserID, string FirstName,string LastName) parameters)
        {
            try
            {
                var profile = context.Profiles.SingleOrDefault(pro => pro.UserID == parameters.UserID);
                if(profile == null)
                {
                    throw new Exception("");
                }
                profile.FirstName = parameters.FirstName;
                profile.LastName = parameters.LastName;
                context.Profiles.Update(profile);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {

                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update profile success"
                });
            }
            catch
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Update profile faild"
                });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPut]
        public IActionResult UpdateProfileForAdmin([FromBody] (Guid UserID, string FirstName, string LastName,Guid ClassID, Guid AuthorizationID) parameters)
        {
            try
            {
                var user = context.Users
                    .Include(user=>user.Profile)
                    .SingleOrDefault(user=>user.UserID== parameters.UserID);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess =false,
                        Message = "Profile Not found"
                    });
                }
                var profile = user.Profile;
                profile.FirstName = parameters.FirstName;
                profile.LastName = parameters.LastName;
                profile.ClassID = parameters.ClassID;
                user.AuthorizationID = parameters.AuthorizationID;
                context.Update(user);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {

                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update profile success"
                });
            }
            catch
            {
                return Ok(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Update profile faild"
                });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public int getNumberOfReviews(Guid UserID)
        {
            var count = context.Reviews.Where(context => context.UserID == UserID).Count();

            return count;
        }
    }
}
