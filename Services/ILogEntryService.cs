using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface ILogEntryService
    {
        public Task<string> CreateDrivingLog(LogEntry logEntry);
        public Task<string> CreateOnDutyLog(LogEntry logEntry);
        public Task<string> CreateOffDutyLog(LogEntry logEntry);
        public Task<string> StopDrivingLog(int userId);
        public Task<string> StopOnDutyLog(int userId);
        public Task<string> StopOffDutyLog(int userId);
        public Task<List<LogEntry>> GetActiveLogEntries(int driverId);
        public Task<TimeSpan> GetTotalDrivingHoursLastWeek(int userId);
        public Task<TimeSpan> GetTotalOnDutyHoursLastWeek(int userId);
        public Task<TimeSpan> GetTotalOffDutyHoursLastWeek(int userId);
        public Task<List<LogEntry>> GetAllLogs(int driverId);

    }
}
