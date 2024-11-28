using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class AdminService(ApplicationDbContext context, IMapper mapper, IPasswordHasher<User> passwordHasher, IConfiguration configuration) : IAdminService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        private readonly IConfiguration _config = configuration;
        public async Task<string> CreateManager(SignUpDTO signUpDTO)
        {
            
            
            var userExists = await context.User.AnyAsync(u => u.Email == signUpDTO.Email);
            if (userExists)
            {
                throw new InvalidOperationException("User already exists.");
            }
            
            else if (signUpDTO.Role == 0 || signUpDTO.Role == 2)
            {
                throw new InvalidOperationException($"Wrong {signUpDTO.Email} Role!");
            }
            

            User newUser = mapper.Map<User>(signUpDTO);
            newUser.EmailVerified = false;
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.FcmDeviceToken = "hello";
            bool duplicate = true;
            var emailCode = UserService.GenerateVerificationToken();
            var allTokens = await context.User.Select(x => x.EmailVerificationToken)
                                                .Where(x=> x != null).ToListAsync();

            //ensures token is unique 
            while(duplicate)
            {
                if(allTokens.Contains(emailCode))
                {
                    emailCode = UserService.GenerateVerificationToken();
                }   
                else
                {
                    duplicate = false;
                }
            }
            newUser.EmailVerificationToken = emailCode;

            //hash password from signupDTO to User for db  
            newUser.Password = _passwordHasher.HashPassword(newUser, signUpDTO.Password);

            await context.User.AddAsync(newUser);
            await context.SaveChangesAsync();
            return newUser.EmailVerificationToken; 
                
        }
      

        public async Task<List<Company>> GetAllComapnies()
        {
            var companies = context.Company.ToList() ?? throw new InvalidOperationException("No company could be found!");
            return companies;
        }

        public async Task<List<User>> GetAllDrivers()
        {
            Console.WriteLine("getting all drivers!");
            var allDrivers = await context.User.Where(u => u.Role == UserRole.Driver).ToListAsync(); 
            return allDrivers;
           
        }

        public async Task<User> GetDriverById(int id)
        {
            var driver = context.User.Where(u =>u.Id == id).FirstOrDefault() ?? throw new InvalidOperationException("No driver can be found with the given Id!");
            return driver;
        }

        public async Task<List<User>> GetDriversByComapnyId(int id)
        {
            var drivers = context.User.Where(u => u.CompanyId == id &&
                                                  u.Role == UserRole.Driver).ToList() ?? throw new InvalidOperationException("No Drivers found!");
            return drivers;
        }

        public async Task<List<LogEntry>> GetLogsByDriverId(int id)
        {
            var logs = context.LogEntry.Where(log => log.UserId == id).ToList() ?? throw new InvalidOperationException("No Logs found!");
            return logs;
        }

        public async Task<bool> CreateCompany(CompanyDTO companyDTO)
        {
            var newCompany = new Company()
            { 
                Name = companyDTO.Name,

            };
            
            var c = await context.Company.Where(c => c.Name == newCompany.Name).FirstOrDefaultAsync();
            if(c != null) throw new InvalidOperationException($"Company with {newCompany.Name} already exists");
            await context.Company.AddAsync(newCompany);
           
            var res = await context.SaveChangesAsync();
            if (res > 0) return true;
            return false;
            //var addedCompany = await context.Company.Where(comp => comp.Name == newCompany.Name ).FirstOrDefaultAsync();
            /*return res <= 0
                ? throw new InvalidOperationException("Company can not be added!")
                : $"Company '{newCompany.Name}' added";
            */
        }

        public async Task<List<User>> GetAllManagers()
        {
            var managers = context.User.Where(u => u.Role == UserRole.Manager).ToList() ?? throw new InvalidOperationException("No Managers found!");
            return managers;
        }

        public async Task<bool> DeleteCompany(int companyId)
        {
            var company = await context.Company.Where(c=> c.Id == companyId).FirstOrDefaultAsync() 
                                            ?? throw new InvalidOperationException("Company not found");
            context.Company.Remove(company);
            var res = await context.SaveChangesAsync();
            if (res > 0) return true;
            return false;
        }

        public async Task<bool> DeleteManager(int userId)
        {
            var user = await context.User.Where(u=> u.Id == userId && u.Role==UserRole.Manager).FirstOrDefaultAsync() 
                                            ?? throw new InvalidOperationException("Manager not found");
            context.User.Remove(user);
            var res = await context.SaveChangesAsync();
            if (res > 0) return true;
            return false;
        }
    }
}
