using AutoMapper;
using Microsoft.Kiota.Abstractions.Extensions;
using System.Data.Entity;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class AdminService(ApplicationDbContext context) : IAdminService
    {
        public async Task<List<Company>> GetAllComapnies()
        {
            var companies = await context.Company.Where(c => c.Id > 0).ToListAsync();
            return companies;
        }

        public async Task<List<User>> GetAllDrivers()
        {
            var allDrivers = await context.User.Where(u => u.Role == UserRole.Driver).ToListAsync(); 
            return allDrivers;
           
        }

        public async Task<User> GetDriverById(int id)
        {
            var driver = await context.User.Where(u =>u.Id == id).FirstOrDefaultAsync() ?? "No driver can be found with the given Id!");
            return driver;
        }

        public async Task<List<User>> GetDriversByComapnyId(int id)
        {
            var drivers = await context.User.Where(u => u.CompanyId == id &&
                                                  u.Role == UserRole.Driver).ToListAsync() ?? throw new InvalidOperationException("No Drivers found!");
            return drivers;
        }

        public async Task<List<LogEntry>> GetLogsByDriverId(int id)
        {
            var logs = await context.LogEntry.Where(log => log.UserId == id).ToListAsync() ?? throw new InvalidOperationException("No Logs found!");
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
