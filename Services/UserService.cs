using AutoMapper;
using Microsoft.AspNetCore.Identity;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(ApplicationDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> CreateUserAsync(SignUpDTO signUpDTO)
        {
            User newDriver = _mapper.Map<User>(signUpDTO);
            //hash password from signupDTO to User for db  
            newDriver.Password = _passwordHasher.HashPassword(newDriver, signUpDTO.Password);

            await _context.User.AddAsync(newDriver);
            await _context.SaveChangesAsync();
            return $"Driver with email {newDriver.Email} was succesfully registered" ;
        }

        public async Task<string> LoginUserAsync(LoginDTO loginDTO)
        {

            return "OK";
        }
    }
}
