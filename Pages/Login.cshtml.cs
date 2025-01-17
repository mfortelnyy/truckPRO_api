using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace truckPRO_api.Pages.Account
{
    private readonly IHttpClientFactory _httpClientFactory;
    public class LoginModel(IHttpClientFactory httpClientFactory) : PageModel
    {
        _httpClientFactory = httpClientFactory;
        
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        //for non-Js users
        public IActionResult OnPost()
        {
            if (IsValidUser(Email, Password))
            {
                return RedirectToPage("/Success");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                return Page();
            }
        }

        private bool IsValidUser(string email, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var loginPayload = new
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginPayload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("https://your-api-domain.com/Login", content);

            if (response.IsSuccessStatusCode)
            {
                //valid if API returns success
                return true;
            }

            // log error
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error during login: {errorDetails}");

            return false;
        }
    }
}