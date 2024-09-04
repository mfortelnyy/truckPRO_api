using System.ComponentModel.DataAnnotations;

namespace truckPRO_api.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<User> Users {get; set;}
    }
}
