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

        public async Task<string> CreateCompany(CompanyDTO companyDTO)
        {
            var newCompany = new Company()
            { 
                Name = companyDTO.Name,

            };
            //Console.WriteLine(newCompany.Name);

            await context.Company.AddAsync(newCompany);
           
            await context.SaveChangesAsync();

            //var addedCompany = await context.Company.Where(comp => comp.Name == newCompany.Name ).FirstOrDefaultAsync();
            return newCompany.Id == 0
                ? throw new InvalidOperationException("Company can not be added!")
                : $"Company with id {newCompany.Id} added";
        }
    }
}
