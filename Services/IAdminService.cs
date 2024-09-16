using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface IAdminService 
    {
        public Task<Company[]> GetAllComapnies();
        public Task<User[]> GetAllDrivers();

        public Task<User[]> GetDriversByComapnyId(int id);
        public Task<User> GetDriverById(int id);
        public Task<LogEntry[]> GetLogsByDriverId(int id);
        public Task<string> 
    }
}
