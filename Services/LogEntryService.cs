using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using truckPRO_api.Data;
using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public class LogEntryService(ApplicationDbContext context) : ILogEntryService
    {
        public async Task<string> CreateOffDutyLog(LogEntry logEntry)
        {
            var userId = logEntry.UserId;

            if (!await HasActiveOnDutyOrDrivingLog(userId))
            {
                throw new InvalidOperationException("Cannot create a Duty Off log. The user does not have an active on-duty or driving log.");
            }
            if (await HasActiveOffDutyLog(userId))
            {
                throw new InvalidOperationException("Cannot create a Duty Off log. The user is currently OFF Duty");
            }
            var activeLogEntryDriving = await context.LogEntry
                                                     .Where(logEntry => userId == logEntry.UserId &&
                                                                        
                                                                     (logEntry.LogEntryType == LogEntryType.OnDuty ||
                                                                     logEntry.LogEntryType == LogEntryType.Driving) &&
                                                                     logEntry.EndTime == null)
                                                     .ToListAsync();
           
            
            //ends all active logs
            foreach (var le in activeLogEntryDriving)
            {
             le.EndTime = DateTime.Now;
             context.LogEntry.Update(le);  
                
            }


            context.Add(logEntry);
            await context.SaveChangesAsync();
            return logEntry.Id.ToString();
        }

        public async Task<string> CreateCycleLog(LogEntry logEntry)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateDrivingLog(LogEntry logEntry)
        {
            var userId = logEntry.UserId;

            //if there is no on duty log 
            if (!await HasActiveOnDuty(userId))
            {
                return "Cannot create a driving log. The user does not have an active on-duty log. ";
            }

            //if there is already a driving log
            if(await HasActiveDriving(userId))
            {
                throw new InvalidOperationException("Cannot create a new driving log. The user already has an active driving log.");

            }

            if (!await ValidLastLogEntryTimeFrame(userId)) throw new InvalidOperationException("Cannot create a driving log. The user does not have an active on-duty log.");
            else
            {
                context.Add(logEntry);
                await context.SaveChangesAsync();
            }

            return logEntry.Id.ToString();

        }

        public async Task<string> CreateOnDutyLog(LogEntry logEntry)
        {

            var userId = logEntry.UserId;

            //check for existing logs that may conflict
            if(await HasActiveOnDutyOrDrivingLog(userId)) 
            {
                throw new InvalidOperationException("User has already has an active on-duty or driving log!");
            }

            //check if on duty is started after the 10 hour off-duty
            if (await IsValidStartTimeAfterBreak(userId))
            {
                throw new InvalidOperationException("On-duty log entry cannot start before completing the required off duty period. (10 hours)");
            }

            //if all checks are passed then create and save logentry to db
            context.LogEntry.Add(logEntry);
            await context.SaveChangesAsync();



            return logEntry.Id.ToString();

        }

        //Checks to validate if the log can be added
        private async Task<bool> HasActiveOnDutyOrDrivingLog(int userId)

        { 
            Console.WriteLine($"userid   {userId}");
            return await context.LogEntry.AnyAsync(logEntry => logEntry.UserId == userId &&
                                                                  (logEntry.LogEntryType == LogEntryType.OnDuty || logEntry.LogEntryType == LogEntryType.Driving) &&
                                                                  logEntry.EndTime == null);
        }

        private async Task<bool> IsValidStartTimeAfterBreak(int userId)

        {
            var allOffDuty = await context.LogEntry
                                          .Where(logEntry => logEntry.UserId == userId && logEntry.LogEntryType == LogEntryType.OffDuty).ToListAsync();

            var activeOffDuty = await context.LogEntry
                                          .Where(logEntry => logEntry.UserId == userId && logEntry.LogEntryType == LogEntryType.OffDuty && logEntry.EndTime == null)
                                          .OrderByDescending(logEntry => logEntry.StartTime)
                                          .FirstOrDefaultAsync();

            //ensures that new drivers can start their shift
            if (activeOffDuty != null)
            {
                var breakDuration = DateTime.Now - activeOffDuty.StartTime;
                if (breakDuration < TimeSpan.FromHours(10))
                {
                    return false; //"Cannot start a new on-duty log. The driver must have at least 10 hours off-duty.";
                }
                else
                {
                    // update the end time of the break log to now
                    activeOffDuty.EndTime = DateTime.Now;
                    context.LogEntry.Update(activeOffDuty);
                    await context.SaveChangesAsync();
                    // break duration is ended when on duty is strated
                    return true; 
                }

            }
            //enusres new drivers can start the log
            else if(activeOffDuty == null && allOffDuty == null) return true;
            else return false;
        }

        //driving log entry can be added after starting the shift (on duty) 
        private async Task<bool> HasActiveOnDuty(int userId)
        {
            var result = await context.LogEntry.Where(u => u.UserId == userId &&
                                                                        u.LogEntryType == LogEntryType.OnDuty&&
                                                                        u.EndTime == null).ToListAsync();
            if (result.Count == 1) return true;
            return false;
            
        }

        //new driving log can not be added because an active driving log is in the system
        private async Task<bool> HasActiveDriving(int userId)
        {
            return await context.LogEntry.AnyAsync(u => u.UserId == userId &&
                                                                        u.LogEntryType == LogEntryType.Driving &&
                                                                        u.EndTime == null);
        }

        //gets the last log entry for the user
        private async Task<bool> ValidLastLogEntryTimeFrame(int userId)
        {
            var lastLogEntry = await context.LogEntry
                                  .Where(u => u.UserId == userId)
                                  .OrderByDescending(u => u.StartTime)
                                  .FirstOrDefaultAsync();

            if (lastLogEntry == null) return false;

            

            if (lastLogEntry.LogEntryType == LogEntryType.OnDuty)
            {
                var LogDuration = lastLogEntry.EndTime - lastLogEntry.StartTime;
                //if on duty log was active more than 14 hours update the endtime and return false
                if (LogDuration > TimeSpan.FromHours(14))
                {
                    //end on duty log and does not allow new driving log
                    lastLogEntry.EndTime = lastLogEntry.StartTime + TimeSpan.FromHours(14);
                    context.Update(lastLogEntry);
                    await context.SaveChangesAsync();
                    return false;
                }
                //if on duty log duration is less than 14 hrs -> allow addding driving log
                else
                {
                    return true;
                }
            }

            //if any other type of logentry is last then no driving is allowed
            return false;
        }

        private async Task<bool> HasActiveOffDutyLog(int userId)
        {
            return await context.LogEntry.AnyAsync(u => u.UserId == userId &&
                                                u.LogEntryType == LogEntryType.OffDuty &&
                                                u.EndTime == null); 
        }


    }
}
