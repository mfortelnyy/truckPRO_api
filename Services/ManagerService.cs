﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
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
    }
}
