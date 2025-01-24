using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using truckPRO_api.DTOs;
using truckPRO_api.Services;
using Microsoft.AspNetCore.Antiforgery;
using truckPRO_api.Models;

namespace truckPRO_api.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IAntiforgery _antiforgery;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IUserService userService, IAntiforgery antiforgery)
        {
            _userService = userService;
            _antiforgery = antiforgery;
        }

        public string AntiforgeryToken { get; private set; }

        public void OnGet()
        {
            // Generate antiforgery token
            AntiforgeryToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Authenticate user and get token
                var token = await AuthenticateUserAsync(Email, Password);

                // Decode token to extract user role
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // Store token in a secure cookie
                HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1) 
                });

                // Navigate based on role
                return role switch
                {
                    nameof(UserRole.Driver) => RedirectToPage("/DriverHome"),
                    nameof(UserRole.Manager) => RedirectToPage("/ManagerHome"),
                    nameof(UserRole.Admin) => RedirectToPage("/AdminHome"),
                    _ => RedirectToPage("/Error")
                };
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                Console.WriteLine($"Login failed: {ex.Message}");
                return Page();
            }
        }

        private async Task<string> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDTO(email, password);
                var response = await _userService.LoginUserAsync(loginDto);
                if (string.IsNullOrEmpty(response))
                {
                    throw new InvalidOperationException("Authentication failed.");
                }
                return ExtractToken(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AuthenticateUserAsync: {ex.Message}");
                throw;
            }
        }

        private string ExtractToken(string responseMessage)
        {
            const string tokenPrefix = "Token: ";
            var tokenStartIndex = responseMessage.IndexOf(tokenPrefix, StringComparison.Ordinal);
            if (tokenStartIndex >= 0)
            {
                return responseMessage.Substring(tokenStartIndex + tokenPrefix.Length).Trim();
            }
            throw new InvalidOperationException("The response does not contain a token in the expected format.");
        }
    }
}