using System.ComponentModel.DataAnnotations;

namespace truckPRO_api.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public LoginDTO(string email, string password){
            Email = email;  
            Password = password;
        }
    }
}
