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
        private readonly IUserValidationService _validationService;


        [BindProperty]
        public SignUpDTO SignUpDTO { get; set; } = new SignUpDTO(); 

        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; } 
        public List<Company> Companies { get; set; }

        public RegisterModel(IUserService userService, IAdminService adminService, IUserValidationService validationService)
        {
            _userService = userService;
            _adminService = adminService;
            _validationService = validationService;



        }

        public async Task OnGetAsync()
        {
            Companies = await _adminService.GetAllComapnies();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var (isValid, errorMessage) = _validationService.Validate(SignUpDTO);
            if (!isValid)
            {
                IsError = true;
                ErrorMessage = errorMessage;
                return Page();
            }

            try
            {
                var result = await _userService.CreateUserAsync(SignUpDTO);
                if (result.Length == 6) // returns email verification code -> success
                {
                    return RedirectToPage("/Success");
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = ex.Message;
            }

            return Page();
        }

    }
}
