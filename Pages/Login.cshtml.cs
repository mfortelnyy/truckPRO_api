using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace truckPRO_api.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await IsValidUserAsync(Email, Password))
            {
                return RedirectToPage("/Success");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                return Page();
            }
        }

        private async Task<bool> IsValidUserAsync(string email, string password)
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
                // Valid if API returns success
                return true;
            }

            // Log error details
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error during login: {errorDetails}");

            return false;
        }
    }
}