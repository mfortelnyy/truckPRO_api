using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace truckPRO_api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;

        public UserService(ApplicationDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _config = configuration;
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

            string token = GenerateJwtToken(driver);

            //otherwise allow sign in 
            return $"User with {driver.Email} succefully signed in. Token: {token}";
        }

        private string GenerateJwtToken(User user)
        {
            //symmetric security key is created using a secret key stored in appsetings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            //the signing credentials are created using the security key and a hmacsha256 algorithms which
            //ensures that the token can’t be tampered with because any modification would result in an invalid signature.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //array of claims - key-value pairs 
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),

                //adds a custom claim representing the user's role
                //allows to enforce role-based authorization
                new Claim("role", user.Role.ToString())
            };

            //generate token with the expiaretion time of 1 hour
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
