using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface IManagerService
    {
        public Task<List<User>> GetAllDriversByCompany(int companyId);
        public Task<List<LogEntry>> GetLogsByDriver(int driverId, int companyId);
        public Task<string> AddDriverToCompany(PendingUser pendingUser);
        public Task<List<PendingUser>> GetPendingDriversByCompanyId(int companyId);
        public Task<string> UpdatePendingDriver(PendingUser pendingUser);
        public Task<List<LogEntry>> GetAllActiveDrivingLogs(int companyId);
        public Task<string> ApproveDrivingLogById(int logEntryId);
        public Task<List<string>> GetImagesOfDrivingLog(int logId);
        public Task<List<User>> GetRegisteredFromPending(int companyId);
        public Task<List<PendingUser>> GetNotRegisteredFromPending(int companyId);
        public Task<List<PendingUser>> GetAllPendingUsers(int companyId);



    }
}
