using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;


        [BindProperty]
        public SignUpDTO SignUpDTO { get; set; } = new SignUpDTO(); 

        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; } 
        public List<Company> Companies { get; set; }

        public RegisterModel(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
            
        }

        public async Task OnGetAsync()
        {
            Companies = await _adminService.GetAllComapnies();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                IsError = true;
                ErrorMessage = "Please fill out all required fields.";
                return RedirectToPage("/Registration");
            }
            else if (ModelState.IsValid)
            {
                if (SignUpDTO.Password != SignUpDTO.ConfirmPassword)
                {
                    IsError = true;
                    ErrorMessage = "Passwords do not match.";
                    return RedirectToPage("/Registration");
                }


                var role = (UserRole)SignUpDTO.Role;
                if ((role == UserRole.Driver || role == UserRole.Manager) && !SignUpDTO.CompanyId.HasValue)
                {
                    IsError = true;
                    ErrorMessage = "CompanyId is required for drivers.";
                    return RedirectToPage("/Registration");
                }

                try
                {
                    // call the SignUp endpoint to register the user
                    var result = await _userService.CreateUserAsync(SignUpDTO);

                    if (result.Length == 6)
                    {
                        return RedirectToPage("/Success");
                    }
                }
                catch (Exception ex)
                {
                    IsError = true;
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Register");
                }
            }
                
            IsError = true;
            ErrorMessage = "ERROR!";
            return RedirectToPage("/Register");

        }
    }
}
