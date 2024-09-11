using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class LogEntryService : ILogEntryService
    {
        private readonly ApplicationDbContext _context;

        // Constructor for dependency injection
        public LogEntryService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<string> CreateBreakLog(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateCycleLog(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateDrivingLog(LogEntry logEntry)
        {
            var userId = logEntry.UserId;
            throw new NotImplementedException();

        }

        public async Task<string> CreateOnDutyLog(LogEntry logEntry)
        {
            var userId = logEntry.UserId;

            //check for existing logs that may conflict
            if(await HasActiveOnDutyOrDrivingLog(userId)) 
            {
                throw new InvalidOperationException("User has already has an active on-duty or driving log!");
            }

            //check if on duty is started after the 10 hour break
            if (await IsValidStartTimeAfterBreak(userId))
            {
                throw new InvalidOperationException("On-duty log entry cannot start before completing the required break period. (10 hours)");
            }

            //if all checks are passed then create and save logentry to db
            _context.LogEntry.Add(logEntry);
            await _context.SaveChangesAsync();

            return logEntry.Id.ToString();

        }

        //Checks to validate if the log can be added
        private async Task<bool> HasActiveOnDutyOrDrivingLog(int userId)
        {
            return await _context.LogEntry.AnyAsync(u => u.UserId == userId &&
                                                                  (u.LogEntryType == LogEntryType.OnDuty || u.LogEntryType == LogEntryType.Driving) &&
                                                                  u.EndTime == null);
        }

        private async Task<bool> IsValidStartTimeAfterBreak(int userId)

        {
            var allBreaks = await _context.LogEntry
                                          .Where(u => u.UserId == userId && u.LogEntryType == LogEntryType.Break).ToListAsync();

            var activeBreak = await _context.LogEntry
                                          .Where(u => u.UserId == userId && u.LogEntryType == LogEntryType.Break && u.EndTime == null)
                                          .OrderByDescending(u => u.StartTime)
                                          .FirstOrDefaultAsync();

            //ensures that new drivers can start their shift
            if (activeBreak != null)
            {
                var breakDuration = DateTime.Now - activeBreak.StartTime;
                if (breakDuration < TimeSpan.FromHours(10))
                {
                    return false; //"Cannot start a new on-duty log. The driver must have at least 10 hours off-duty.";
                }
                else
                {
                    // update the end time of the break log to now
                    activeBreak.EndTime = DateTime.Now;
                    _context.LogEntry.Update(activeBreak);
                    await _context.SaveChangesAsync();
                    // break duration is ended when on duty is strated
                    return true; 
                }

            }
            //enusres new drivers can start the log
            else if(allBreaks.Count == 0) return true;
            else return false;
        }

    }
}
