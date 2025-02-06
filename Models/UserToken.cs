using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace truckPRO_api.Models
{
    [Table("UserTokens")]
    public class UserToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        public DateTime? LastUsed { get; set; } 

        public bool IsRevoked { get; set; } = false; 
        public virtual User User { get; set; }
    }
}