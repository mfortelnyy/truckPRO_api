using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using truckPRO_api.DTOs;
using truckPRO_api.Services;


namespace truckPRO_api.Controllers
{
    public class AdminController(IAdminService adminService, IUserService userService) : Controller
    {
        [HttpPost]
        [Route("signUpManager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SignUpManager([FromBody] SignUpDTO SignUpDTO)
        {
            if ((SignUpDTO.Role == 1 || SignUpDTO.Role == 2) && !SignUpDTO.CompanyId.HasValue)
            {
                return BadRequest("CompanyId is required for drivers.");
            }

            //if model is not valid then the request is bad - 400
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string result = await userService.CreateUserAsync(SignUpDTO);
            if (result != null) return Ok(result);
            return BadRequest();

        }


        [HttpGet]
        [Route("/getAllCompanies")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllCompanies()
        {
            var companies = await adminService.GetAllComapnies();
            if (companies == null) return Conflict("No Companies found!");
            return Ok(companies);
        }

        [HttpGet]
        [Route("/adm/getAllDrivers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDrivers()
        {
            var drivers = await adminService.GetAllDrivers();
            if (drivers == null) return NotFound("No Drivers found!");
            return Ok(drivers);
        }


        [HttpGet]
        [Route("/adm/getDriverById")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDriverById([FromQuery] int userId)
        {
            var driver = await adminService.GetDriverById(userId);
            if (driver == null) return NotFound("No Driver found!");
            return Ok(driver);
        }

        [HttpGet]
        [Route("/adm/getDriversByCompanyId")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDriversByCompanyId([FromQuery] int companyId)
        {
            var drivers = await adminService.GetDriversByComapnyId(companyId);
            if (drivers == null) return NotFound("No Drivers found!");
            return Ok(drivers);
        }


        [HttpGet]
        [Route("/adm/getLogsByDriverId")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLogsByDriverId([FromQuery] int driverId)
        {
            var logs = await adminService.GetLogsByDriverId(driverId);
            if (logs == null) return NotFound("No Logs found!");
            return Ok(logs);
        }

        [HttpPost]
        [Route("/adm/createCompany")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDTO companyDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var res = await adminService.CreateCompany(companyDTO);
            if (res == null) return NotFound("Company can not be created at this moment!");
            return Ok(res);
        }

        [HttpGet]
        [Route("/adm/getAllManagers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllManagers()
        {
            var res = await adminService.GetAllManagers();
            if (res == null) return NotFound("Managers can not be found!");
            return Ok(res);
        }

    }
}
