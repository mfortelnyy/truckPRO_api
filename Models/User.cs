using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace truckPRO_api.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }
        //foreign key -> company
        public int CompanyID { get; set; }
        public Company Company { get; set; }



    }
}
