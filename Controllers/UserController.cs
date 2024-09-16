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
    public class UserController(IUserService userService, IEmailService emailService) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly IEmailService _emailService = emailService;

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
            try
            {
                var role = (UserRole)SignUpDTO.Role;
                if ((role == UserRole.Admin || role == UserRole.Manager) && !SignUpDTO.CompanyId.HasValue)
                {
                    return BadRequest("CompanyId is required for drivers.");
                }

                if (role != UserRole.Admin && role != UserRole.Manager)
                {
                    //if model is not valid then the request is bad - 400
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }


                    string emailVerificarionToken = await _userService.CreateUserAsync(SignUpDTO);
                    await _emailService.SendEmailAsync(email: SignUpDTO.Email, subject: "Registration", message: "You are registered", emailVerificarionToken);
                    if (emailVerificarionToken != null) return Ok(emailVerificarionToken);
                    return BadRequest();
                }
                //ensures admin can not be created
                else return Unauthorized($"User with role {role.ToString()} can not be created");
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


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


        [HttpPost]
        [Route("/verifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string emailToken)
        {
            try
            {
                Console.WriteLine($"email token: {emailToken}");

                var result = await _userService.VerifyEmail(emailToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }
       



    }
}
