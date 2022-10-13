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
        [Authorize]
        [HttpGet]
        public IActionResult GetInformation([Required] Guid UserID)
        {
            try
            {
                var profile = context.Profiles
                    .Include(pro => pro.Classification)
                    .FirstOrDefault(pro => pro.UserID == UserID);
                if (profile == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Cannot Get User Information!"
                    });
                }
                var profileDTO = _mapper.Map<Data.Profile, ProfileDTO>(profile);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message="Get user information success",
                    Data = profileDTO
                });
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
        public IActionResult UpdateProfile([FromBody] ProfileDTO profileDTO)
        {
            try
            {
                var profile = context.Profiles.SingleOrDefault(pro => pro.ProfileID == profileDTO.ProfileID);
                if(profile == null)
                {
                    throw new Exception("");
                }
                profile.FirstName = profileDTO.FirstName;
                profile.LastName = profile.LastName;
                profile.Email = profileDTO.Email;
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
                return Ok(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Update profile faild"
                });
            }
        }
    }
}
