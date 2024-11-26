using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace truckPRO_api.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(2), MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2), MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Phone]
        [Required]
        public string Phone { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }

        //foreign key -> company
        [ForeignKey("CompanyId")]
        public int? CompanyId { get; set; }

        [MinLength(3)]
        public string FcmDeviceToken{ get; set; }

        public DateTime CreatedAt { get; set; }

        public Company Company { get; set; }

        public bool EmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
  
    }
}
