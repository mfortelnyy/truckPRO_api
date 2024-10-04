using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(SignUpDTO sDTO);
        Task<string> LoginUserAsync(LoginDTO lDTO);
        Task<string> VerifyEmail(string emailToken);
        Task<string> UpdatePassword(int driverId, string oldPassword, string newPassword);
    }
}
