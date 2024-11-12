using System;
using System.Threading.Tasks;

namespace truckPRO_api.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateDrivingRecordsPdfAsync(int driverId, DateTime startDate, DateTime endDate);
    }
}
