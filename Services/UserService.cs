using truckPRO_api.Data;
using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAsync(SignUpDTO signUpDTO)
        {
            await _context.User.AddAsync(signUpDTO);
            await _context.SaveChangesAsync();
            return true ;
        }

        public async Task<bool> LoginUserAsync(LoginDTO loginDTO)
        {

        }
    }
}
