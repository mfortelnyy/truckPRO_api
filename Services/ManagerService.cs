
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

        public async Task<string> AddDriverToCompany(PendingUser pendingUser)
        {
            
            var pUser = await context.PendingUser.AddAsync(pendingUser);
            await context.SaveChangesAsync();

            if (pUser == null) throw new InvalidOperationException("Driver can not be created!");
            //Console.WriteLine($"Driver with id {pUser.Entity.Id} added to company {pUser.Entity.CompanyId} by manager with result {pUser.Entity.Email}");
            return $"Driver with id {pUser.Entity.Id} added to company {pUser.Entity.CompanyId} by manager"; 
        }

        public async Task<List<PendingUser>> GetPendingDriversByCompanyId(int companyId)
        {
            var pUsers = await context.PendingUser.Where(u => u.CompanyId==companyId).ToListAsync();
            if (pUsers == null) throw new InvalidOperationException("Drivers can not be found!");
            return pUsers;
        }

        public async Task<string> UpdatePendingDriver(PendingUser pendingUser)
        {
            context.Update(pendingUser);
            var res = await context.SaveChangesAsync(true);
            //if (res == null) throw new InvalidOperationException("PendingDriver can not be updated");
            return "Pending Driver succefully updated!";

        }

        public async Task<List<LogEntry>> GetAllActiveDrivingLogs(int companyId)
        {
            var drivingLogs = await context.LogEntry
                .Include(log => log.User)
                .Where(predicate: log => log.User.CompanyId == companyId && 
                                                                log.LogEntryType == LogEntryType.Driving &&
                                                                log.EndTime == null).ToListAsync();

            if (drivingLogs == null || drivingLogs.Count == 0) throw new InvalidOperationException("No active drivers driving");
            return drivingLogs;
        }

        public async Task<string> ApproveDrivingLogById(int logEntryId)
        {
            var logEntry = await context.LogEntry.FirstOrDefaultAsync(log => log.Id == logEntryId);
            if (logEntry == null) throw new InvalidOperationException("Log could not be found!");
            logEntry.IsApprovedByManager = true;
            await context.SaveChangesAsync(true);
            return "Log was successfully approved!";
        }

        
    }
}
