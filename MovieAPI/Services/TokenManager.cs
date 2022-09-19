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
        private readonly MovieAPIDbContext context;
        private readonly ILogger logger;
        public TokenManager(MovieAPIDbContext movieAPIDbContext, ILogger iLogger)
        {
            context = movieAPIDbContext;
            logger = iLogger;
        }
        public TokenModel GenerateAccessToken(User user)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.StartMethod());
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(AppSettings.SecretKey!);
            var expires = DateTime.UtcNow.AddHours(1);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(user.UserID), user.UserID.ToString()),
                    new Claim(nameof(user.AuthorizationID), user.AuthorizationID.ToString())
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = RandomText.RandomCharacter();
            var tokenData = new Token
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = expires,
                User = user
            };
            try
            {
                context.Tokens?.Add(tokenData);
                context.SaveChanges();
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess());
            }
            catch(Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError(ex.ToString()));
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.EndMethod());
            }
            
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.EndMethod());
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public string DecodeToken(TokenModel tokenModel)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.StartMethod());
            var accessToken = tokenModel.AccessToken!.Replace("Bearer ", "");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var list = token.Claims.ToList();
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.EndMethod());
            return null;
        }
    }
}
