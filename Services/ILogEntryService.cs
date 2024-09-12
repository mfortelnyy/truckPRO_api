using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface ILogEntryService
    {
        public Task<string> CreateDrivingLog(LogEntry logEntry);
        public Task<string> CreateOnDutyLog(LogEntry logEntry);
        public Task<string> CreateOffDutyLog(LogEntry logEntry);
        public Task<string> CreateCycleLog(LogEntry logEntry);



    }
}
