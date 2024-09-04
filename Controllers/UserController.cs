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
        public IActionResult Login(LoginDTO loginDTO)
        {
            return Ok(200);

            //to do - add dbContext in program.cs and scaffold a controller
            /*var user = dbContext.Users.FirstOrDefault(x => x.userName && x.Password == loginDTO.Password);
            if (user != null) {
                return Ok(user);
            }
            */
        }
    }
}
