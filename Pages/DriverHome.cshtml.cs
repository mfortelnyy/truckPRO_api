using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using truckPRO_api.Services;
using truckapi.DTOs;

namespace truckPRO_api.Pages
{
    public class DriverHome : PageModel
    {
        private readonly ILogEntryService _driverService;
        private readonly IUserService _userService;
        public string DriverName { get; set; } = string.Empty;
        public string TotalOnDutyHours { get; set; } = string.Empty;
        public string TotalDrivingHours { get; set; } = string.Empty;
        public string TotalOffDutyHours { get; set; } = string.Empty;

        public DriverHome(ILogEntryService driverService, IUserService userService)
        {
            _driverService = driverService ?? throw new ArgumentNullException(nameof(driverService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> OnGet()
        {
            // Retrieve and validate token
            string token = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Authorization token is missing.");
            }

            // Decode token to extract user ID
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int driverId))
            {
                return BadRequest("Invalid token. User ID is missing or invalid.");
            }

            // Retrieve user details
            UserDTO user = await _userService.GetUserById(driverId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            DriverName = user.FirstName;

            // Fetch weekly summary
            TotalOnDutyHours = (await _driverService.GetTotalOnDutyHoursLastWeek7days(driverId)).ToString(@"hh\:mm\:ss");
            TotalDrivingHours = (await _driverService.GetTotalDrivingHoursLastWeek(driverId)).ToString(@"hh\:mm\:ss");
            TotalOffDutyHours = (await _driverService.GetTotalOffDutyHoursLastWeek(driverId)).ToString(@"hh\:mm\:ss");

            return Page();
        }
    }
}