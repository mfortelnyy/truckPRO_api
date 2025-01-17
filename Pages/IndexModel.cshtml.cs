using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourAppNamespace.Pages
{
    public class IndexModel : PageModel
    {
        public string WelcomeMessage { get; private set; }

        public void OnGet()
        {
            WelcomeMessage = "Welcome to Truck Check Pro - Smarter Fleet Management!";
        }
    }
}