using AutoMapper;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Education.Classes.Item.Assignments.Item.Submissions.Item.Return;
using truckPro_api.DTOs;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class LoEntryService(ApplicationDbContext context, IMapper mapper) : ILogEntryService
    {
        private readonly IMapper _mapper = mapper;

        public async Task<string> CreateOnDutyLog(LogEntry logEntry)
        {
            //if new user then limit checks are not performed
            var newUser = await IsNewUser(logEntry.UserId);

            if(await HasActiveOnDutyCycle(logEntry.UserId))
            {
                return "You can not start a new On Duty Log Entry.\nYou have an active On Duty Log!";
            }

            // Ensure there is a valid off-duty cycle before the on-duty shift
            //HasValidOffDutyCycle end current off duty if rest period is satisfied
            if (newUser == false && !await HasValidOffDutyCycle(logEntry.UserId))
            {
                return "You need to take a break for at least 10 hours before starting a new on-duty shift.";
            }

            logEntry.StartTime = DateTime.UtcNow;
            logEntry.LogEntryType = LogEntryType.OnDuty;

            context.LogEntry.Add(logEntry);
            await context.SaveChangesAsync();

            return "On-duty log created successfully."; 
        }
        
        public async Task<string> CreateDrivingLog(LogEntry logEntry)
        {
            if(await HasActiveDrivingCycle(logEntry.UserId))
            {
                return "You can not start a new Driving Log Entry.\nYou have an active Driving Log!";
            }

            //if there is no active on duty log then create one
            if(!await HasActiveOnDutyCycle(logEntry.UserId))
            {
                LogEntry newOnDutyLog = new LogEntry
                {
                    UserId = logEntry.UserId,
                    StartTime = DateTime.UtcNow,
                    EndTime = null,
                    LogEntryType = LogEntryType.OnDuty,
                    ImageUrls = null,
                };
                var res = await CreateOnDutyLog(newOnDutyLog);
                if(!res.Contains("successfully"))
                {
                    throw new InvalidOperationException("Something went wrong. Please, try again later!");
                }
            }

            //establish relationship between the on duty log (parent) and the driving log
            var activeOnDutyLog = await GetActiveOnDutyLog(logEntry.UserId);
            logEntry.ParentLogEntryId = activeOnDutyLog.Id;

            //check if the driver has exceeded the daily driving limit of 11 hours
            if (await HasExceededDailyDrivingLimit(logEntry))
            {
                return "Driving limit exceeded.\nYou cannot drive for more than 11 hours today.";
            }

            //check if the driver has exceeded the 14-hour on-duty limit
            if (await HasExceededOnDutyLimit(logEntry.UserId))
            {
                return "On-duty limit exceeded.\nYou cannot be on-duty for more than 14 hours today.";
            }

            logEntry.StartTime = DateTime.UtcNow;
            logEntry.LogEntryType = LogEntryType.Driving;
            context.LogEntry.Add(logEntry);
            await context.SaveChangesAsync();

            return "Driving Log Started successfully.";
        }


        public async Task<string> CreateOffDutyLog(LogEntry logEntry)
        {
            if(await HasActiveOffDutyCycle(logEntry.UserId))
            {
                return "You can not start a new Off Duty Log Entry.\nYou have an active Off Duty Log!";
            }

            if(await HasActiveOnDutyCycle(logEntry.UserId))
            {
                await StopOnDutyLog(logEntry.UserId);
            }

            logEntry.StartTime = DateTime.UtcNow;
            logEntry.LogEntryType = LogEntryType.OffDuty;
            context.LogEntry.Add(logEntry);
            await context.SaveChangesAsync();
            
            return "Off Duty Log Started successfully.";

        }

        //In context of Off duty cycle break is sleep
        //For On duty cycle break is just a break (30 min after 8hrs)
        public async Task<string> CreateBreakLog(LogEntry logEntry)
        {
            var hasActiveOffDuty = await HasActiveOffDutyCycle(logEntry.UserId);
            var hasActiveOnDuty = await HasActiveOnDutyCycle(logEntry.UserId);

            if(await HasActiveBreakCycle(logEntry.UserId))
            {
                return "You can not start a new Break Log Entry.\nYou have an active Break Log!";
            }

            if(hasActiveOffDuty && hasActiveOnDuty)
            {
                return "Something went wrong. Please try again later!";
            }

            if(hasActiveOffDuty)
            {
                var activeOffDuty = await GetActiveOffDutyLog(logEntry.UserId);
                logEntry.StartTime = DateTime.UtcNow;
                logEntry.LogEntryType = LogEntryType.Break;
                logEntry.ParentLogEntryId = activeOffDuty!.Id;
                context.LogEntry.Add(logEntry);
                await context.SaveChangesAsync();
                return $"Sleep Log Started successfully!";
            }
            else if(hasActiveOnDuty)
            {
                var activeOnDuty = await GetActiveOnDutyLog(logEntry.UserId);
                logEntry.StartTime = DateTime.UtcNow;
                logEntry.LogEntryType = LogEntryType.Break;
                logEntry.ParentLogEntryId = activeOnDuty!.Id;
                context.LogEntry.Add(logEntry);
                await context.SaveChangesAsync();
                return $"Break Log Started successfully!";
            }

            return "Something went wrong. Please try again later!";
        }

       

        public async Task<LogEntryParent?> GetActiveLogEntries(int driverId)
        {
            //return await context.LogEntry.Where(logEntry => logEntry.UserId == driverId && logEntry.EndTime == null).ToListAsync();
            //add DTO OnDuty/OffDuty main that contains a list of other logentries with parentid the same as id of onduty/offduty
            var activeOnDutyLog = await GetActiveOnDutyLog(driverId);
            var activeOffDutyLog = await GetActiveOffDutyLog(driverId);
            var parentLogentryActive = new LogEntryParent();
            //holds the main active log - off duty or on duty with all the children logs

            if (activeOnDutyLog == null && activeOffDutyLog == null)
            {
                return null;
            }
            else if (activeOnDutyLog != null && activeOffDutyLog != null)
            {
                return null;
            }
            else if(activeOffDutyLog != null && activeOnDutyLog == null)
            {
                parentLogentryActive = _mapper.Map<LogEntryParent>(activeOffDutyLog);

                //get all child logs for 'Off Duty'
                var childrenLogs = await context.LogEntry.Where(log => log.UserId == driverId 
                                && log.ParentLogEntryId == activeOffDutyLog.Id && log.EndTime == null)
                                .OrderBy(log => log.StartTime)
                                .ToListAsync();

                parentLogentryActive.ChildLogEntries = childrenLogs;
            }
            else if(activeOnDutyLog != null && activeOffDutyLog == null)
            {
                parentLogentryActive = _mapper.Map<LogEntryParent>(activeOnDutyLog);

                //get all child logs for 'On Duty'
                var childrenLogs = await context.LogEntry.Where(log => log.UserId == driverId
                                && log.ParentLogEntryId == activeOnDutyLog.Id && log.EndTime == null)
                                .OrderBy(log => log.StartTime)
                                .ToListAsync();
                parentLogentryActive.ChildLogEntries = childrenLogs;
            }
            return parentLogentryActive;
        }

        public async Task<List<LogEntryParent>> GetAllLogs(int driverId)
        {
            var parentLogentries = new List<LogEntryParent>();
            var allParentLogEntries = await context.LogEntry.Where(log => log.UserId == driverId
                                && (log.LogEntryType == LogEntryType.OnDuty || log.LogEntryType == LogEntryType.OffDuty))
                                .OrderBy(log=>log.StartTime).ToListAsync();


            foreach(var parentLogEntry in allParentLogEntries)
            {
                var childrenLogs = await context.LogEntry.Where(log => log.ParentLogEntryId == parentLogEntry.Id && log.UserId == driverId)
                                                   .OrderBy(log => log.StartTime).ToListAsync();

                var newParentLog = _mapper.Map<LogEntryParent>(parentLogEntry);
                newParentLog.ChildLogEntries = childrenLogs;
                parentLogentries.Add(newParentLog);
            }
            return parentLogentries;
            
        }

        public async Task<TimeSpan> GetTotalDrivingHoursLastWeek(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<TimeSpan> GetTotalOffDutyHoursLastWeek(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<TimeSpan> GetTotalOnDutyHoursLastWeek(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> NotifyManagers(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StopDrivingLog(int userId)
        {
            //locate active on duty 
            var activeOnDuty = await GetActiveLogEntries(userId);
            if (activeOnDuty == null)
            {
                return "No active On Duty Log found.";
            }
            //use active on duty as parent to locate the current driving log
            var activeDrivingLog = await context.LogEntry.Where(log => log.UserId == userId
                                && log.ParentLogEntryId == activeOnDuty.Id
                                && log.LogEntryType == LogEntryType.Driving
                                && log.EndTime == null).FirstOrDefaultAsync();
            if(activeDrivingLog == null)
            {
                return "No active Driving Log found.";
            }
            //if found driving log -> end it and save changes
            activeDrivingLog.EndTime = DateTime.UtcNow;
            context.Update(activeDrivingLog);
            await context.SaveChangesAsync();
            return "Driving Log Stopped successfully!";

        }

        public async Task<string> StopOffDutyLog(int userId)
        {
            LogEntry? activeOffDutyLog = await GetActiveOffDutyLog(userId);
            if (activeOffDutyLog == null)
            {
                return "No active Off Duty Log found.";
            }

            // if found then update it's endtime
            activeOffDutyLog.EndTime = DateTime.Now;
            context.Update(activeOffDutyLog);
            await context.SaveChangesAsync();

            return $"Off Duty Log Stopped successfully";
        }

        public async Task<string> StopOnDutyLog(int userId)
        {
            var activeOnDutyLog = await GetActiveOnDutyLog(userId);
            if (activeOnDutyLog == null)
            {
                return "No active On Duty Log found.";
            }

            var activePerOnDuty = await context.LogEntry
                            .Where(l => l.UserId == userId 
                            && l.ParentLogEntryId == activeOnDutyLog.Id 
                            && l.EndTime == null).ToListAsync();
            foreach(var log in activePerOnDuty)
            {
                log.EndTime = DateTime.UtcNow;
                context.Update(log);
                
            }
            activeOnDutyLog.EndTime = DateTime.UtcNow;
            context.Update(activeOnDutyLog);
            await context.SaveChangesAsync();
            return $"On Duty Log Stopped successfully as well as {activePerOnDuty.Count} other related logs";

        }



        // Limit checks - helper funcitons


        // 1 - Limit checks for daily driving limit
        public async Task<bool> HasExceededDailyDrivingLimit(LogEntry le)
        {
            //locate current driver's 'On Duty' log
            var activeOnDutyLog = await GetActiveOnDutyLog(le.UserId) ?? throw new InvalidOperationException("No current On Duty Log");

            //retrieve all driving logs for the current on duty log
            var DrivingLogsPerOnDuty = await context.LogEntry
                .Where(log => log.UserId == le.UserId && log.ParentLogEntryId == activeOnDutyLog.Id && log.ParentLogEntryId == le.ParentLogEntryId && log.LogEntryType == LogEntryType.Driving && log.EndTime != null)
                .ToListAsync();

            //calculate total driving hours for the current On Duty log
            double totalDrivingHoursPerOnDuty = DrivingLogsPerOnDuty.Sum(log =>
                (log.EndTime!.Value - log.StartTime).TotalHours);

            //check if limit exceeded 
            return totalDrivingHoursPerOnDuty > 11;
        }

        // 2 - Limit check for on-duty hours (14 hours max)
        public async Task<bool> HasExceededOnDutyLimit(int userId)
        {
            //locate current driver's 'On Duty' log
            var activeOnDutyLog = await context.LogEntry
                                        .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OnDuty && log.EndTime == null)
                                        .FirstOrDefaultAsync() ?? throw new InvalidOperationException("No current On Duty Log");

            DateTime currentDateTimeUtc = DateTime.UtcNow;
            TimeSpan currentOnDutyHours = activeOnDutyLog.StartTime - currentDateTimeUtc;

            return currentOnDutyHours > TimeSpan.FromHours(14);
        }

        //LAST CHECK
        public async Task<bool> HasValidOffDutyCycle(int userId)
        {
            //get first off duty with null end time = endtime == null
            var currentOffDutyCycleLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OffDuty && log.EndTime == null)
                .FirstOrDefaultAsync(); 
            
            LogEntry? lastOffDutyLog = null;
            //if there is no current of duty log then check in completed off duty logs
            if(currentOffDutyCycleLog == null)
            {
              lastOffDutyLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OffDuty && log.EndTime != null)
                .OrderByDescending(log => log.EndTime)
                .FirstOrDefaultAsync();
            }

            //if both are null something wrong since new users with not logs wouldn't be checked
            if (lastOffDutyLog == null && currentOffDutyCycleLog == null)
            {
                return false;
            }
            else if (lastOffDutyLog == null && currentOffDutyCycleLog != null)
            {
                //calculate duration for of the current of duty log
                var offDutyDuration = DateTime.UtcNow - currentOffDutyCycleLog.StartTime;
                var hasValidRest = offDutyDuration.TotalHours >= 10;
                //THIS IS THE LAST CHECK SO IF THREASHOLD FOR REST REACHED - STOP OFF DUTY WHEN CREATE ON DUTY IS RECEIVED
                if (hasValidRest)
                {
                    //end off duty and save changes
                    var res = await StopOffDutyLog(userId);
                    if (res.Contains("successfully"))
                    {
                        return true;
                    }     
                }
                return hasValidRest;
            }
            else if (lastOffDutyLog != null && currentOffDutyCycleLog == null)
            {
                //calculate duration of last off duty log
                var offDutyDuration = lastOffDutyLog.EndTime!.Value - lastOffDutyLog.StartTime;
                return offDutyDuration.TotalHours >= 10;
            }
            return false; 
        }

        public async Task<bool> IsNewUser(int userId)
        {
            var lastLogEntry = await context.LogEntry
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.StartTime)
                .FirstOrDefaultAsync();

            return lastLogEntry == null;
        }

        public async Task<bool> HasActiveOnDutyCycle(int userId)
        {
            var activeOnDutyLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OnDuty && log.EndTime == null)
                .FirstOrDefaultAsync();

            return activeOnDutyLog != null;
        }

        public async Task<bool> HasActiveDrivingCycle(int userId)
        {
            var activeDrivingLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.Driving && log.EndTime == null)
                .FirstOrDefaultAsync();

            return activeDrivingLog != null;
        }

        public async Task<LogEntry> GetActiveOnDutyLog(int userId)
        {
            var activeOnDutyLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OnDuty && log.EndTime == null)
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException("No current On Duty Log");

            return activeOnDutyLog;
        }

        public async Task<LogEntry?> GetActiveOffDutyLog(int userId)
        {
             return await context.LogEntry.Where(u => u.UserId == userId &&
                                                u.LogEntryType == LogEntryType.OffDuty &&
                                                u.EndTime == null)
                                            .OrderByDescending(u => u.StartTime)
                                            .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveOffDutyCycle(int userId)
        {
            var activeOffDutyLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OffDuty && log.EndTime == null)
                .FirstOrDefaultAsync();

            return activeOffDutyLog != null;
        }

        public async Task<bool> HasActiveBreakCycle(int userId)
        {
            var activeBreakLog = await context.LogEntry
                .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.Break && log.EndTime == null)
                .FirstOrDefaultAsync();

            return activeBreakLog != null;
        }


    }
}