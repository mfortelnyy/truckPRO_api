using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using truckPRO_api.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using truckPRO_api.Services;
using Microsoft.AspNetCore.Antiforgery;



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
            //generate antiforgery token
            AntiforgeryToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        }


        public async Task<IActionResult> OnPostAsync()
        {
            //send http request to auth user and get token with decoded info about user
            var token = await AuthenticateUserAsync(Email, Password);
            if (!string.IsNullOrEmpty(token))
            {
                //decode token to extract user role
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                //store token in session or cookie
                HttpContext.Session.SetString("AuthToken", token);

                //navigate based on role
                return role switch
                {
                    "Driver" => RedirectToPage("/DriverHome"),
                    "Manager" => RedirectToPage("/ManagerHome"),
                    "Admin" => RedirectToPage("/AdminHome"),
                    _ => RedirectToPage("/Error")
                };
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                return Page();
            }
        }

        private async Task<string> AuthenticateUserAsync(string email, string password)
        {
            
            //var client = _httpClientFactory.CreateClient();
            LoginDTO loginDto = new LoginDTO(email, password);
        
            /*
            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("https://truckcheck.org:443/Login", content);
            
            Console.WriteLine(response.Content);
            if (response.IsSuccessStatusCode)
            {
                //raw response string
                var responseData = await response.Content.ReadAsStringAsync();

                //get token
                var token = ExtractToken(responseData);

                return token;
            }
            */

            //log error details
            //var errorDetails = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Error during login: {errorDetails}");
            
            try{
              var res = await _userService.LoginUserAsync(loginDto);
              if(res != null)
              {
                return ExtractToken(res);
              }
              return res;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private string ExtractToken(string responseMessage)
        {
            const string tokenPrefix = "Token: ";
            //.log($"{responseMessage}");
            var tokenStartIndex = responseMessage.IndexOf(tokenPrefix, StringComparison.Ordinal);

            if (tokenStartIndex >= 0)
            {
                // Extract the token
                return responseMessage.Substring(tokenStartIndex + tokenPrefix.Length).Trim();
            }

            throw new InvalidOperationException("The response does not contain a token in the expected format.");
        }
    }
}