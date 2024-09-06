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
        public IActionResult Login([FromBody] LoginDTO LoginDTO)
        {
            //Console.WriteLine(LoginDTO.Email);
            return Ok(200);

            /*var user = dbContext.Users.FirstOrDefault(x => x.userName && x.Password == loginDTO.Password);
            if (user != null) {
                return Ok(user);
            }
            */
        } 


        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody]SignUpDTO SignUpDTO)
        {
            //if model is not balid then the request is bad - 400
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(SignUpDTO);
            if (user != null) return Ok(user);
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
