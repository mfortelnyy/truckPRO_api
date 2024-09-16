using AutoMapper;
using Microsoft.Kiota.Abstractions.Extensions;
using System.Data.Entity;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class AdminService(ApplicationDbContext context, IMapper mapper) : IAdminService
    {
        public async Task<List<Company>> GetAllComapnies()
        {
            var companies = context.Company.Where(c => c.Id > 0).ToList();
            return companies;
        }

        public async Task<List<User>> GetAllDrivers()
        {
            var allDrivers = context.User.Where(u => u.Role == UserRole.Driver).ToList(); 
            return allDrivers;
           
        }

        public async Task<User> GetDriverById(int id)
        {
            var driver = context.User.Where(u =>u.Id == id).FirstOrDefault();
            return driver;
        }

        public async Task<List<User>> GetDriversByComapnyId(int id)
        {
            var drivers = context.User.Where(u => u.Id == id &&
                                                  u.Role == UserRole.Driver).ToList();
            return drivers;
        }

        public async Task<List<LogEntry>> GetLogsByDriverId(int id)
        {
            var logs = context.LogEntry.Where(log => log.UserId == id).ToList();
            return logs;
        }

        public async Task<string> CreateCompany(CompanyDTO companyDTO)
        {
            return "";
        }
    }
}
