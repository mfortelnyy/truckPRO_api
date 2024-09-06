using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(SignUpDTO sDTO);
        Task<bool> LoginUserAsync(LoginDTO lDTO);
    }
}
