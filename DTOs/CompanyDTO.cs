using System.ComponentModel.DataAnnotations;
using truckPRO_api.Models;

namespace truckPRO_api.DTOs
{
    public class CompanyDTO
    {

        [Required]
        [MinLength(2), MaxLength(50)]
        public string Name { get; set; }

        //public List<User> users { get; set; }   
    }
}