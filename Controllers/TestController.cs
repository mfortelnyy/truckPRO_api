using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace truckPRO_api.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            Console.WriteLine(token);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }

            var isValid = ValidateToken(token);

            if (isValid)
            {
                return Ok("Token is valid");
            }
            else
            {
                return Unauthorized("Token is invalid");
            }
        }

        [HttpGet("driver-data")]
        [Authorize(Roles = "Driver")] // Requires JWT and the "Driver" role
        public IActionResult GetDriverData()
        {
            return Ok(new { message = "This is a driver-specific secured endpoint." });
        }

        [HttpGet("token-claims")]
        public IActionResult GetTokenClaims()
        {
            // Extract the token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validate the token and extract the claims
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "JwtIssuer",
                    ValidAudience = "JwtAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("QoyLLM8SxXaUfYMJKT7svrVlAgpJD04d"))
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                var claims = principal.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Unauthorized($"Token validation failed: {ex.Message}");
            }
        }


        private static bool ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                Console.WriteLine($"I'm here with token {token}");
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "JwtIssuer",  
                    ValidAudience = "JwtAudience",  
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("QoyLLM8SxXaUfYMJKT7svrVlAgpJD04d"))  
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }

}
