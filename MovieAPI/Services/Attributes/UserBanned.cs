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
    public class UserBanned : ValidationAttribute
    {
        public UserBanned() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("Khang");
            var UserID = (context.HttpContext.User.Identity as ClaimsIdentity).FindFirst("UserID").Value;
            using MovieAPIDbContext movieAPIDbContext = new MovieAPIDbContext();
            var user = movieAPIDbContext.Users.Find(Guid.Parse(UserID));
            if (user == null)
            {
                Console.WriteLine("Khang");
                context.Result = new NotFoundObjectResult(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }
            if (user.Status == false)
            {
                Console.WriteLine("True");
                context.Result = new NotFoundObjectResult(new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Your user has been banned"
                });
            }
        }
    }
}
