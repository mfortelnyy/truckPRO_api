using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO LoginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string result = await _userService.LoginUserAsync(LoginDTO);
            if (result != null) return Ok(result);
            return BadRequest();
        }


        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO SignUpDTO)
        {
            if (SignUpDTO.Role != 0)
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

                string result = await _userService.CreateUserAsync(SignUpDTO);
                if (result != null) return Ok(result);
                return BadRequest();
            }
            //ensures admin can not be created
            else return Unauthorized("admin can not be created");

        }

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

            string result = await _userService.CreateUserAsync(SignUpDTO);
            if (result != null) return Ok(result);
            return BadRequest();

        }



    }
}
