
using Microsoft.EntityFrameworkCore;
using truckPRO_api.Data;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class ManagerService(ApplicationDbContext context) : IManagerService
    {
        public async Task<List<User>> GetAllDriversByCompany(int CompanyId)
        {
            var AllDrivers = await context.User.Where(u =>  u.CompanyId == CompanyId).ToListAsync();
            return AllDrivers;
        }

        public async Task<List<LogEntry>> GetLogsByDriver(int driverId, int companyId)
        {
            //ensure that companyid for manager and user is the same
            var AllLogs = await context.LogEntry.Where(u=> u.UserId == driverId &&
                                                              u.User.CompanyId == companyId).ToListAsync();
            return AllLogs;
        }
    }
}
