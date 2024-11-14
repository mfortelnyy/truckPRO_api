using System;
using System.Threading.Tasks;
using truckPRO_api.Models;

namespace truckPRO_api.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateDrivingRecordsPdfAsync(int driverId, DateTime startDate, DateTime endDate, List<LogEntryType> selectedLogTypes);
    }
}
