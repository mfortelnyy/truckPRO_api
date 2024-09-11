using truckPRO_api.DTOs;

namespace truckPRO_api.Services
{
    public class LogEntryService : ILogEntryService
    {
        Task<string> ILogEntryService.CreateBreakLog(BreakLogEntryDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<string> ILogEntryService.CreateCycleLog(CycleLogEntryDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<string> ILogEntryService.CreateDrivingLog(DrivingLogEntryDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<string> ILogEntryService.CreateOnDutyLog(OnDutyLogEntryDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
