using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MovieAPI.Services.Attributes
{
    public class UserBanned : AuthorizeAttribute, IAuthorizationFilter
    {
        public UserBanned() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var UserID = (context.HttpContext.User.Identity as ClaimsIdentity).FindFirst("UserID").Value;
            using MovieAPIDbContext movieAPIDbContext = new MovieAPIDbContext();
            var user = movieAPIDbContext.Users.Find(Guid.Parse(UserID));
            if (user == null)
            {
                context.Result = new NotFoundObjectResult(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }
            if (user.Status == false)
            {
                context.Result = new ObjectResult("Forbidden")
                {
                    StatusCode = 403,
                    Value = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Your user has been banned"
                    }
                };
                return;
            }
        }
    }
}
