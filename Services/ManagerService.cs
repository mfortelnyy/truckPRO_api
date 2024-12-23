﻿
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using truckPro_api.DTOs;
using truckPRO_api.Data;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class ManagerService(ApplicationDbContext context, IMapper mapper) : IManagerService
    {
        IMapper _mapper = mapper;
        public async Task<List<User>> GetAllDriversByCompany(int CompanyId)
        {
            var AllDrivers = await context.User.Where(u =>  u.CompanyId == CompanyId && u.Role == UserRole.Driver).ToListAsync();
            return AllDrivers;
        }

        
        public async Task<List<LogEntryParent?>> GetLogsByDriver(int driverId)
        {
            var user = await context.User.Where(u => u.Id == driverId).FirstOrDefaultAsync();
            int? cid = user.CompanyId;
            
            // parent logs (on duty and off duty) that are in-progress 
            var inProgressLog = await context.LogEntry
                .Where(log => log.UserId == driverId 
                && log.EndTime == null
                && (log.LogEntryType == LogEntryType.OnDuty || log.LogEntryType == LogEntryType.OffDuty))
                .OrderByDescending(log => log.StartTime)
                .FirstOrDefaultAsync();

            var childrenInProgressLogs = await context.LogEntry
                    .Where(log => log.UserId == driverId && log.ParentLogEntryId == inProgressLog!.Id).ToListAsync();

            LogEntryParent inProgressParentLog = _mapper.Map<LogEntryParent>(inProgressLog);

            inProgressParentLog.ChildLogEntries = childrenInProgressLogs;


            // logs that are completed  and order them by endtime descending
            var finishedParentLogs = await context.LogEntry
                .Where(log => log.UserId == driverId && log.EndTime != null
                                && (log.LogEntryType == LogEntryType.OnDuty || log.LogEntryType == LogEntryType.OffDuty))
                .OrderByDescending(log => log.EndTime)
                .ToListAsync();
            List<LogEntryParent?> finishedParentLogsList = [];
            // combine in-progress logs with finished logs
            finishedParentLogsList.Add(inProgressParentLog);
            foreach (var finishedParentLog in finishedParentLogs)
            {
                var childrenFinishedLogs = await context.LogEntry
                    .Where(log => log.UserId == driverId && log.ParentLogEntryId == finishedParentLog!.Id).ToListAsync();
                var mappedParentLog = _mapper.Map<LogEntryParent>(finishedParentLog);
                mappedParentLog.ChildLogEntries = childrenFinishedLogs;
                finishedParentLogsList.Add(mappedParentLog);
            }
            
            return finishedParentLogsList;
        }


        public async Task<string> AddDriverToCompany(PendingUser pendingUser)
        {
            var pUserExists = await context.PendingUser.Where(pu => pu.Email == pendingUser.Email).FirstOrDefaultAsync();
            if(pUserExists != null) throw new InvalidOperationException($"Driver can not be created! Pending User already exists {pUserExists.Email}");
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

        public async Task<List<LogEntry?>> GetAllActiveDrivingLogs(int companyId)
        {
            var activeDrivingLogs = await context.LogEntry
                .Include(log => log.User)
                .Where(predicate: log => log.User.CompanyId == companyId && 
                                                                log.LogEntryType == LogEntryType.OnDuty &&
                                                                log.EndTime == null).ToListAsync() ?? throw new InvalidOperationException("No Active Logs at this moment!");

            List<LogEntry?> activeDrivingLogsPerCompany = [];
            foreach (var log in activeDrivingLogs)
            {                
                var activeDrivingLog = await context.LogEntry.Where(l => l.ParentLogEntryId == log.Id &&
                             log.EndTime == null).FirstOrDefaultAsync();
                if (activeDrivingLog != null)
                { 
                    activeDrivingLogsPerCompany.Add(activeDrivingLog);    
                }                       
            }
            return activeDrivingLogsPerCompany;
        }

        public async Task<string> ApproveDrivingLogById(int logEntryId)
        {
            var logEntry = await context.LogEntry.FirstOrDefaultAsync(log => log.Id == logEntryId && log.LogEntryType == LogEntryType.Driving);
            if (logEntry == null) throw new InvalidOperationException("Log could not be found!");
            logEntry.IsApprovedByManager = true;
            var res = await context.SaveChangesAsync(true);
            return res != 0 ? "Log was successfully approved!" : "";
        }

        public async Task<List<string>?> GetImagesOfDrivingLog(int logId)
        {
            var logentry = await context.LogEntry.FirstOrDefaultAsync(log => log.Id == logId);
            if (logentry == null) throw new InvalidOperationException("Log could not be found!");
            if (logentry.LogEntryType != LogEntryType.Driving) throw new InvalidOperationException("Log prvoided is not a driving Log");
            return logentry.ImageUrls;
            
        }

        public async Task<List<User>> GetRegisteredFromPending(int companyId) 
        {
            //join user and pendingUser tables and filter by copanyid and verifiedEmail
            var regisredUsers =  await context.PendingUser
                     .Join(
                          context.User,
            pendingUser => pendingUser.Email, // key selector from PendingUser
            user => user.Email, // key selector from User
            (pendingUser, user) => new { PendingUser = pendingUser, User = user } // result selector
                     )
                    .Where(result => result.PendingUser.CompanyId == companyId && result.User.Email != null) // filter by company ID and verfified email
                    .Select(result => result.User) // select the user information
                    .ToListAsync();

            if (regisredUsers == null) throw new InvalidOperationException("No registered users found!");
            return regisredUsers;
        }

        public async Task<List<PendingUser>> GetNotRegisteredFromPending(int companyId)
        {
            //get the list of registered user emails
            var registeredEmails = await context.User
                .Where(u => u.CompanyId == companyId && u.Email != null)
                .Select(u => u.Email) //list of onlyemails
                .ToListAsync();

            //filter by confirming company id and the email is not in registered emails list
            var notRegisteredPendingUsers = await context.PendingUser
                    .Where(pu => pu.CompanyId == companyId && !registeredEmails.Contains(pu.Email)) 
                    .ToListAsync() ?? throw new InvalidOperationException("All users where registered!");
            
            return notRegisteredPendingUsers;
        }

        public async Task<List<PendingUser>> GetAllPendingUsers(int companyId)
        {
            var pUsers = await context.PendingUser.Where(pu => pu.CompanyId == companyId).ToListAsync() ?? throw new InvalidOperationException("All users where registered!");
            return pUsers;
        }

        public async Task<int> DeletePendingUser(int userId)
        {
            var pUser = await context.PendingUser.Where(pu => pu.Id == userId).FirstOrDefaultAsync();
            if (pUser != null)
            {
                context.Remove(pUser);
                var res = await context.SaveChangesAsync();
                return res;
            }
            return 0;
            
        }
    }
}
