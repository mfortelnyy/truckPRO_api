using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using truckPRO_api.Models;

namespace truckapi.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public UserRole Role { get; set; }

        public int? CompanyId { get; set; }

        public bool EmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
  
    }
    
}