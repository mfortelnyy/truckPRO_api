using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface IManagerService
    {
        public Task<List<User>> GetAllDriversByCompany(int companyId);
        public Task<List<LogEntry>> GetLogsByDriver(int DriverId);

    }
}
