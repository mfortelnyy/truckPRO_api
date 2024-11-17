using System;
using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public interface IUserValidationService
    {
        public (bool isValid, string errorMessage) Validate(SignUpDTO dto);
    }

}
