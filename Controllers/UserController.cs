using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using truckPRO_api.DTOs;
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
            //Console.WriteLine(LoginDTO.Email);
            //return Ok(200);
            //if model is not valid then the request is bad - 400
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
        public async Task<IActionResult> SignUp([FromBody]SignUpDTO SignUpDTO)
        {
            //if model is not valid then the request is bad - 400
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string result = await _userService.CreateUserAsync(SignUpDTO);
            if (result != null) return Ok(result);
            return BadRequest();

        }

        /*
        [HttpGet]
        [Route("Welcome")]
        public IActionResult Welcome()
        {
            this.ViewData["Message"] = "Under Development.";
            return this.View();
        }
        */


    }
}
