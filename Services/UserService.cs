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
            //retrieve driver from the db by the email provided
            var driver = _context.User.FirstOrDefault(dr => dr.Email == loginDTO.Email);
            
            //if there is no driver with indicated email then return message
            if (driver == null)
            {
                return "Invalid username or password.";
            }

            //verify provided password against password stored in db
            var paswordVerification = _passwordHasher.VerifyHashedPassword(driver,driver.Password, loginDTO.Password);

            //if user provided incorrect password then return same message
            if(paswordVerification == PasswordVerificationResult.Failed)
            {
                return "Invalid username or password.";
            }
            //otherwise allow sign in 
            return $"User with {driver.Email} succefully signed in";
        }
    }
}
