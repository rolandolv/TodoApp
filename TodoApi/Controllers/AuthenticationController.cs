using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public record AuthenticationData(string? Username, string? Password);

        public record UserData(int Id, string FirstName, string LastName, string UserName);

        [HttpPost("token")]
        [AllowAnonymous]
        public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
        {
            var user = ValidateCredentials(data);

            if (user is null)
            {
                return Unauthorized();
            }

            var token = GenerateToken(user);

            return Ok(token);
        }

        private string GenerateToken(UserData user)
        {
            var secretKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Authentication:SecretKey")));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
            claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

            var token = new JwtSecurityToken(
                _configuration.GetValue<string>("Authentication:Issuer"),
                _configuration.GetValue<string>("Authentication:Audience"),
                claims, 
                DateTime.UtcNow, 
                DateTime.UtcNow.AddMinutes(5), 
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserData? ValidateCredentials(AuthenticationData data)
        {
            // THIS IS NOT PRODUCTION CODE - REPLACE WITH A CALL TO YOUR AUTH IMPLEMENTATION
            if (CompareValues(data.Username, "rlazcares") && CompareValues(data.Password, "Test123"))
            {
                return new UserData(1, "Rolando", "Lazcares", data.Username);
            }
            
            if (CompareValues(data.Username, "sstorm") && CompareValues(data.Password, "Test123"))
            {
                return new UserData(1, "Sue", "Storm", data.Username);
            }

            return null;
        }

        private bool CompareValues(string? actual, string expected)
        {
            if (actual is not null && actual.Equals(expected))
            {
                return true;
            }

            return false;
        }
    }
}
