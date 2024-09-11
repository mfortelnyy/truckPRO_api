using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public interface ILogEntryService
    {
        public Task<string> CreateDrivingLog(DrivingLogEntryDTO dto);
        public Task<string> CreateOnDutyLog(OnDutyLogEntryDTO dto);
        public Task<string> CreateBreakLog(BreakLogEntryDTO dto);
        public Task<string> CreateCycleLog(CycleLogEntryDTO dto);



    }
}
