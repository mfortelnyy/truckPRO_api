using System.ComponentModel.DataAnnotations;

namespace truckPRO_api.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2), MaxLength(50)]
        public string? Name { get; set; }
        //one-to-many relationship with user
        public ICollection<User>? Users { get; set; }
    }
}
