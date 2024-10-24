using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
                    await _emailService.SendEmailAsync(email: SignUpDTO.Email, subject: "Registration", message: $"You are registered!\nHere is your verification code: {emailVerificarionToken}");
                    if (emailVerificarionToken != null) return Ok(emailVerificarionToken);
                    return BadRequest();
                }
                //ensures admin can not be created
                else return Unauthorized($"User with role {role.ToString()} can not be created");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [HttpPost]
        [Route("verifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string emailToken)
        {
            try
            {
                //Console.WriteLine($"email token: {emailToken}");

                var result = await _userService.VerifyEmail(emailToken);
                return Ok(new {message = result});
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return Conflict(new {message = ex.Message});
            }

        }

        //[EnableCors("AllowPATCH")]
        [HttpPost]
        [Route("updatePassword")]
        [Authorize(Roles = "Manager,Driver")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO updDTO)
        {
            //if (newPassword != confirmPassword) return BadRequest(new { message = "Password did not match!"});
            try
            {
                if(updDTO.oldPassword == updDTO.newPassword) return BadRequest(new {message = "New password can not be the same as old password!"});
                var requestUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                //Console.WriteLine($"oldpassword from controller: {updDTO.oldPassword}");
                var res = await _userService.UpdatePassword(requestUserId, updDTO.oldPassword, updDTO.newPassword);
                return Ok(new {message = res});
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }


        [HttpPost]
        [Route("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromForm] String email)
        {
            try
            {
                var tempPassword = await _userService.ForgetPassword(email);
                await emailService.SendEmailAsync(email, "Temporary Password", $"Your temporary password is: {tempPassword} \nPlease Sign in and update your password!"); 
                return Ok(new {message = "Temp Password sent successfully!"});
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpGet]
        [Route("getUserbyToken")]
        [Authorize(Roles = "Manager, Driver, Admin")]
        public async Task<IActionResult> GetUserByToken([FromForm] String email)
        {
            try
            {
                var requestUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId").Value);
                var userDTO = await _userService.GetUserById(requestUserId);
               
                return Ok(userDTO);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

    }
}
