using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;  
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace truckPRO_api.Services
{
    public class UserService(ApplicationDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher, IConfiguration configuration) : IUserService
    {

        private static readonly Random _random = new Random();
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        private readonly IConfiguration _config = configuration;

        public async Task<string> CreateUserAsync(SignUpDTO signUpDTO)
        {
                       
            var userExists = await _context.User.AnyAsync(u => u.Email == signUpDTO.Email);
            var pendingDriver = await _context.PendingUser.FirstOrDefaultAsync(pd => pd.Email == signUpDTO.Email);
            if (userExists)
            {
                throw new InvalidOperationException("User already exists.");
            }
            
            else if (pendingDriver == null || signUpDTO.Role == 0 || signUpDTO.Role == 1)
            {
                throw new InvalidOperationException($"Email {signUpDTO.Email} was not added by the manager.");
            }

            else if(signUpDTO.CompanyId != pendingDriver!.CompanyId)
            {
                throw new InvalidOperationException("Company Id does not match with the one provided by the manager.");
            }
            
            User newUser = _mapper.Map<User>(signUpDTO);
            bool duplicate = true;
            var emailCode = GenerateVerificationToken();
            var allTokens = await _context.EmailVerificationTokens.Select(x => x.Token).ToListAsync();

            //ensures token is unique 
            while(duplicate)
            {
                if(allTokens.Contains(emailCode))
                {
                    emailCode = GenerateVerificationToken();
                }   
                else
                {
                    duplicate = false;
                }
            }

            EmailVerificationToken emailVerificationToken = new EmailVerificationToken
            {
              UserId = newUser.Id,
              Token = emailCode,
              EmailVerified = true,
              Expiration = DateTime.UtcNow.AddHours(6),
              IsUsed = false,
            };
            
            await _context.EmailVerificationTokens.AddAsync(emailVerificationToken);

            newUser.EmailVerificationToken = emailVerificationToken;

            //hash password from signupDTO to User for db  
            newUser.Password = _passwordHasher.HashPassword(newUser, signUpDTO.Password);
            newUser.FcmDeviceToken = "hellodriver";
            newUser.CreatedAt = DateTime.UtcNow;
            await _context.User.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser.EmailVerificationToken.Token; 
                //$"{newUser.Role} with email {newUser.Email} was succesfully registered" ;
        }
        public static string GenerateVerificationToken()
        {
            return _random.Next(100000, 999999).ToString();
        }

        public async Task<string> LoginUserAsync(LoginDTO loginDTO)
        {
            //retrieve driver from the db by the email provided
            var driver =  await _context.User.FirstOrDefaultAsync(dr => dr.Email == loginDTO.Email);
            
            //if there is no driver with indicated email then return message
            if (driver == null)
            {
                throw new Exception("Invalid username or password.");
            }

            //verify provided password against password stored in db
            var paswordVerification = _passwordHasher.VerifyHashedPassword(driver,driver.Password, loginDTO.Password);

            //if user provided incorrect password then return same message
            if(paswordVerification == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid username or password.");
            }

            //update Activity Status to Active
            driver.Status = ActivityStatus.Active;
            await _context.SaveChangesAsync();

            string token = await GenerateJwtToken(driver);

            //otherwise allow sign in 
            return $"User with {driver.Email} succefully signed in. Token: {token}";
        }

        public async Task<string> VerifyEmail(int userId, string emailToken)
        {
            var userEmailToken = await _context.EmailVerificationTokens.FirstOrDefaultAsync(evt => evt.UserId == userId);

            //Console.WriteLine($"email token: {emailToken}");
            if (userEmailToken == null || userEmailToken.Token != emailToken)
            {
                throw new InvalidOperationException("Invalid token!");
            }
            userEmailToken.EmailVerified = true;
            await _context.SaveChangesAsync();
            return "Email verfied Sucefully!";
        }

        public async Task<String> SaveNewVerificationCode(int userId)
        {
            var userEmailToken = await _context.EmailVerificationTokens.FirstOrDefaultAsync(evt => evt.UserId == userId);
            
            if(userEmailToken.EmailVerified = true) return "Email is Already Verified!";
            String newCode = GenerateVerificationToken();
        
            userEmailToken.Token = newCode;
            userEmailToken.Expiration = DateTime.Now.AddHours(6);
            await _context.SaveChangesAsync();
            return newCode;
        }

        public async Task<string> UpdatePassword(int userId, string oldPassword, string newPassword)
        {
            //Console.WriteLine($"userid: {userId}  oldpasswd: {oldPassword}  newpass {newPassword}");
            var driver = await context.User.Where(u => u.Id == userId).FirstOrDefaultAsync() ??
                       throw new InvalidOperationException("No driver found!");

            var paswordVerification = _passwordHasher.VerifyHashedPassword(driver, driver.Password, oldPassword);

            //if user provided incorrect password then return same message
            if(paswordVerification == PasswordVerificationResult.Failed)
            {
                //Console.WriteLine("old pswd is incorredct");
                throw new InvalidOperationException("Old Password is incorrect");
            }
            else
            {
                Console.WriteLine("Driver found! to upd passwrd");
                driver.Password = _passwordHasher.HashPassword(driver, newPassword);
                await _context.SaveChangesAsync();
                return "Password for user successfully updated!";
            }
        }


        public static string GenerateTemporaryPassword(int length = 12)
        {
            const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string DigitChars = "0123456789";
            const string SpecialChars = "!@#$%^&*()_-+=<>?"; 
            
            // Combine the character sets for password generation
            string allChars = LowercaseChars + UppercaseChars + DigitChars + SpecialChars;
            StringBuilder password = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[1];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(randomBytes);
                    int randomIndex = randomBytes[0] % allChars.Length;
                    password.Append(allChars[randomIndex]);
                }
            }
            return password.ToString();
        }

    
        private async Task<String> GenerateJwtToken(User user)
        {
            //Console.WriteLine(_config["Jwt:Key"]);
            //symmetric security key is created using a secret key stored in appsetings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("QoyLLM8SxXaUfYMJKT7svrVlAgpJD04d"));

            //the signing credentials are created using the security key and a hmacsha256 algorithms which
            //ensures that the token can’t be tampered with because any modification would result in an invalid signature.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //token is valid for 24hrs
            var expirationTime = DateTime.UtcNow.AddHours(24);

            //Console.WriteLine(user.Role.ToString());
            //array of claims - key-value pairs 
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),

                //adds a custom claim representing the user's role
                //allows to enforce role-based authorization
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("companyId", user.CompanyId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expirationTime).ToUnixTimeSeconds().ToString()) //expiration timestamp
            };

            //generate token with the expiaretion time of 24 hours
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"], 
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials
                );
            

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Check if the user already has a valid token in the database
            var existingUserToken = await _context.UserTokens
                .Where(ut => ut.UserId == user.Id)
                .FirstOrDefaultAsync();

            if (existingUserToken != null)
            {
                // If an active token exists and is not expd and not revoked, return the existing token instead of generating a new one
                if(existingUserToken.Expiration > DateTime.UtcNow && existingUserToken.IsRevoked == false)
                {
                    // Update last datetime used
                    existingUserToken.LastUsed = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return existingUserToken.Token;
                }

                // If token is either expd or revoked than update the token and its info
                else
                {
                    existingUserToken.Token = tokenString;
                    existingUserToken.Expiration = expirationTime;
                    existingUserToken.LastUsed = DateTime.UtcNow;
                    existingUserToken.IsRevoked = false;
                    await _context.SaveChangesAsync();
                    return tokenString;
                }

            }

            // If no valid token exists, create and store a new one
            var newUserToken = new UserToken
            {
                UserId = user.Id,
                Token = tokenString,
                Expiration = expirationTime,
                LastUsed = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.UserTokens.Add(newUserToken);
            await _context.SaveChangesAsync();

            return tokenString;
        }

        public async Task<string> ForgetPassword(String email)
        {
            //Console.WriteLine($"user found: {email}");
            var user = await context.User.Where(u => u.Email == email).FirstOrDefaultAsync() ??
                       throw new InvalidOperationException("No user found!");
            //Console.WriteLine($"user found: {user.Email}");
            String tempPassword = GenerateTemporaryPassword();
            //Console.WriteLine($"temp pswd: {tempPassword}");

            user.Password = _passwordHasher.HashPassword(user, tempPassword);
            await _context.SaveChangesAsync();
            return tempPassword;
        }

        public async Task<UserDTO> GetUserById(int userId)
        {
            var user = await _context.User.Where(u => u.Id == userId).FirstOrDefaultAsync() ??
                       throw new InvalidOperationException("No user found!");
            return _mapper.Map<UserDTO>(user);
            
        }

        public async Task<string> UpdateDeviceToken(int userId, string fcmDeviceToken)
        {
            var user = _context.User.Where(u => u.Id == userId).FirstOrDefault() ?? throw new InvalidOperationException("No user found!");
            user.FcmDeviceToken = fcmDeviceToken;
            var res = await _context.SaveChangesAsync();
            if(res == 0)
            {
                throw new InvalidOperationException("No user found!");
            }
            return "Successfully Update Device Token!";
        }

        public Task<List<string>> GetManagerFcmTokensAsync(int companyId)
        {
            var res = _context.User.Where(u => u.CompanyId == companyId && u.Role == UserRole.Manager).Select(u => u.FcmDeviceToken).Where(f => f != null).ToListAsync() ?? throw new InvalidOperationException("No Managers Signed in!");
            return res;
        }

        public async Task<bool> DeleteAccount(int userId)
        {
            var user = await _context.User.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
                return true;

            }
            else
            {
                return false;
            }
        }
        public async Task<string> LogoutUserAsync(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Set status to Inactive on logout
            user.Status = ActivityStatus.Inactive;
            await _context.SaveChangesAsync();

            return "User successfully logged out.";
        }
    }
}
