using Docker.DotNet.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class LoEntryService(ApplicationDbContext context) : ILogEntryService
    {
        public async Task<string> CreateOnDutyLog(LogEntry logEntry)
        {
            //if new user then just start
            var newUser = await IsNewUser(logEntry.UserId);

            // // Check if the driver has exceeded the 14-hour on-duty limit
            // if (newUser == false && await HasExceededOnDutyLimit(logEntry.UserId))
            // {
            //     return "On-duty limit exceeded. You cannot be on-duty for more than 14 hours today.";
            // }

            // Ensure there is a valid off-duty cycle before the on-duty shift
            if (newUser == false && !await HasValidOffDutyCycle(logEntry.UserId))
            {
                return "You need to take a break for at least 10 hours before starting a new on-duty shift.";
            }

            logEntry.StartTime = DateTime.UtcNow;
            logEntry.LogEntryType = LogEntryType.OnDuty;

            context.LogEntry.Add(logEntry);
            await context.SaveChangesAsync();

            return "On-duty log created successfully."; ;
        }
        
        public async Task<string> CreateDrivingLog(LogEntry logEntry)
        {
            // 1 - if the driver has exceeded the daily driving limit of 11 hours
            if (await HasExceededDailyDrivingLimit(logEntry.UserId))
            {
                return "Driving limit exceeded. You cannot drive for more than 11 hours today.";
            }

            // 2 - if the driver has exceeded the 14-hour on-duty limit
            if (await HasExceededOnDutyLimit(logEntry.UserId))
            {
                return "On-duty limit exceeded. You cannot be on-duty for more than 14 hours today.";
            }

            // 3 - ensure a valid break (off-duty cycle) before the driving shift
            if (await HasValidOffDutyCycle(logEntry.UserId))
            {
                return "You need to take a break for at least 10 hours before starting a new driving shift.";
            }

            // 4. Set the start time and log type
            logEntry.StartTime = DateTime.UtcNow;
            logEntry.LogEntryType = LogEntryType.Driving;

            // 5. Add the log entry to the database
            //context.LogEntries.Add(logEntry);
            await context.SaveChangesAsync();

            return "Driving log created successfully.";
        }


        public async Task<string> CreateOffDutyLog(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }

       

        public async Task<List<LogEntry>> GetActiveLogEntries(int driverId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LogEntry>> GetAllLogs(int driverId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public async Task<string> StopOffDutyLog(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StopOnDutyLog(int userId)
        {
            throw new NotImplementedException();
        }


        // Limit checks - helper funcitons


        // 1 - Limit checks for daily driving limit
        public async Task<bool> HasExceededDailyDrivingLimit(int userId)
        {
            //locate current driver's 'On Duty' log
            var activeOnDutyLog = await context.LogEntry
                                        .Where(log => log.UserId == userId && log.LogEntryType == LogEntryType.OnDuty && log.EndTime == null)
                                        .FirstOrDefaultAsync() ?? throw new InvalidOperationException("No current On Duty Log");

            //retrieve all driving logs for the current on duty log
            var activeDrivingLogsPerOnDuty = await context.LogEntry
                .Where(log => log.UserId == userId && log.ParentLogEntryId == activeOnDutyLog.Id && log.LogEntryType == LogEntryType.Driving && log.EndTime == null)
                .ToListAsync();

            //calculate total driving hours for the current On Duty log
            double totalDrivingHoursPerOnDuty = activeDrivingLogsPerOnDuty.Sum(log =>
                log.EndTime != null 
                    ? (log.EndTime.Value - log.StartTime).TotalHours 
                    : (DateTime.Now - log.StartTime).TotalHours);

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
                return offDutyDuration.TotalHours >= 10;
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

    }
}