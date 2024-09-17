﻿using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface IManagerService
    {
        public Task<List<User>> GetAllDriversByCompany(int companyId);
        public Task<List<LogEntry>> GetLogsByDriver(int driverId, int companyId);
        public Task<string> AddDriverToCompany(PendingUser pendingUser);
        public Task<List<PendingUser>> GetPendingUsersByCompanyId(int companyId);
        public Task<string> UpdatePendingDriver(PendingUser pendingUser);


    }
}
