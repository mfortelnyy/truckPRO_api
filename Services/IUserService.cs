using truckapi.DTOs;
using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(SignUpDTO sDTO);
        Task<string> LoginUserAsync(LoginDTO lDTO);
        Task<string> VerifyEmail(int userId, string emailToken);
        Task<string> UpdatePassword(int driverId, string oldPassword, string newPassword);
        Task<string> ForgetPassword(String email);
        Task<UserDTO> GetUserById(int userId);
        Task<string> SaveNewVerificationCode(int userId);
        Task<string> UpdateDeviceToken(int userId, string fcmDeviceToken);
        Task<List<string>> GetManagerFcmTokensAsync(int companyId);
        Task<bool>  DeleteAccount(int userId);

    }
}
