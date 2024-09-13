using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using truckPRO_api.Models;

namespace truckPRO_api.DTOs
{
    public class SignUpDTO
    {

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = " The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [EnumDataType(typeof(UserRole))]
        public int Role { get; set; }
        [Required]
        public string Phone { get; set; }
        public int? CompanyId { get; set; }
    }
}
