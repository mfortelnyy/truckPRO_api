using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace truckPRO_api.Pages
{
    public class Index : PageModel
    {
        public string WelcomeMessage { get; private set; }

        public void OnGet()
        {
            WelcomeMessage = "Welcome to Truck Check Pro - Smarter Fleet Management!";
        }
    }
}