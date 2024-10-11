using Microsoft.EntityFrameworkCore;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class AdminService(ApplicationDbContext context) : IAdminService
    {
        public async Task<List<Company>> GetAllComapnies()
        {
            var companies = context.Company.ToList() ?? throw new InvalidOperationException("No company could be found!");
            return companies;
        }

        public async Task<List<User>> GetAllDrivers()
        {
            Console.WriteLine("getting all drivers!");
            var allDrivers = context.User.Where(u => u.Role == UserRole.Driver).ToList(); 
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
