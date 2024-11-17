using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class UserValidationService : IUserValidationService
    {
        public (bool isValid, string errorMessage) Validate(SignUpDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.ConfirmPassword))
                return (false, "Password fields cannot be empty.");

            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match.");

            var role = (UserRole)dto.Role;
            if ((role == UserRole.Driver || role == UserRole.Manager) && !dto.CompanyId.HasValue)
                return (false, "CompanyId is required for drivers and managers.");
            Console.WriteLine("User is valid");
            return (true, string.Empty);
        }
    }
}
