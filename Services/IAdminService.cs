using truckPRO_api.DTOs;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface IAdminService 
    {
        public Task<List<Company>> GetAllComapnies();
        public Task<List<User>> GetAllDrivers();

        public Task<List<User>> GetDriversByComapnyId(int id);
        public Task<User> GetDriverById(int id);
        public Task<List<LogEntry>> GetLogsByDriverId(int id);
        public Task<string> CreateCompany(CompanyDTO companyDTO);

        public Task<List<User>> GetAllManagers();
        public Task<bool> DeleteCompany(int companyId);
       
    }
}
