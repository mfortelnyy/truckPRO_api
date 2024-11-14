using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IPdfService _pdfService;

        public ReportsController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpGet("getdrivingRecordsPDF")]
        [Authorize(Roles = "Driver, Admin, Manager")]
        public async Task<IActionResult> GetDrivingRecordsPdf([FromQuery] DateTime startDate, DateTime endDate, int driverId, List<LogEntryType> selectedLogTypes)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            //var userId = int.Parse(User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            try
            {
                var pdfBytes = await _pdfService.GenerateDrivingRecordsPdfAsync(driverId, startDate, endDate, selectedLogTypes);
                return File(pdfBytes, "application/pdf", $"DrivingRecords_{driverId}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest("No Driving Records available!");
            }
            
        }
    }
}
