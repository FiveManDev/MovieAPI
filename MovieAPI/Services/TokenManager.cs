using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MovieAPI.Services
{
    public class TokenManager
    {
        public static TokenModel GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(AppSettings.SecretKey!);
            var expires = DateTime.UtcNow.AddHours(1);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(user.UserID), user.UserID.ToString()),
                    new Claim(nameof(user.AuthorizationID), user.AuthorizationID.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Authorization?.AuthorizationName??""),
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public static UserModel DecodeToken(string AccessToken)
        {
            var accessToken = AccessToken!.Replace("Bearer ", "");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var list = token.Claims.ToList();
            var userModel = new UserModel
            {
                UserID=list[0].Value,
                AuthorizationID=list[1].Value,
            };
            return userModel;
        }
    }
}
