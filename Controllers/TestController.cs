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
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

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

        private bool ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "JwtIssuer",  // Replace with your issuer
                    ValidAudience = "JwtAudience",  // Replace with your audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("QoyLLM8SxXaUfYMJKT7svrVlAgpJD04d"))  
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                // Optionally log or inspect the exception for more details
                return false;
            }
        }
    }

}
