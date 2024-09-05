using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace truckPRO_api.Models
{
    [Table("Company")]
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
