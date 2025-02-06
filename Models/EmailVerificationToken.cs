using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace truckPRO_api.Models
{
    [Table("EmailVerificationTokens")]
    public class EmailVerificationToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }
        public bool EmailVerified { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        public bool IsUsed { get; set; } = false;

        public virtual User User { get; set; }
    }
}