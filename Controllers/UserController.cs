using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using truckPRO_api.Models;

namespace truckPRO_api.Controllers
{
    public class UserController : Controller
    {
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
        public IActionResult SignUp([FromBody]SignUpDTO SignUpDTO)
        {
            //Console.WriteLine(SignUpDTO.Email);
            return Ok(200);

            
        }
    }
}
