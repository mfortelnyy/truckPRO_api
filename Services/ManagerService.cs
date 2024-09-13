
using truckPRO_api.Data;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class ManagerService(ApplicationDbContext context) : IManagerService
    {
        public async Task<List<User>> GetAllDriversByCompany(int CompanyId)
        {
            var AllDrivers = context.User.Where(u =>  u.CompanyId == CompanyId).ToList();
            return AllDrivers;
        }

        public async Task<List<LogEntry>> GetLogsByDriver(int DriverId)
        {
            var AllLogs = context.LogEntry.Where(u=> u.UserId == DriverId).ToList();
            return AllLogs;
        }
    }
}
