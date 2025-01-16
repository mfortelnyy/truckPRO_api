using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace truckPRO_api.Pages
{
    public class SuccessModel : PageModel
    {
        public string Message { get; set; }

        public IActionResult OnGet()
        {
            if (TempData["RegistrationSuccess"] == null)
            {
                //redirect to the registration page if no successful registration flag exists
                return RedirectToPage("/Register");
            }

            //set the success message
            Message = "Your registration was successful. Please check your email to verify your account.";

            //keep flag for subsequent requests
            TempData.Keep("RegistrationSuccess");

            return Page();

        }
    }
}