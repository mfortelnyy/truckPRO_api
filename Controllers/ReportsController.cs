using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetDrivingRecordsPdf(DateTime startDate, DateTime endDate)
        {
            var userId = int.Parse(User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var pdfBytes = await _pdfService.GenerateDrivingRecordsPdfAsync(userId, startDate, endDate);
            return File(pdfBytes, "application/pdf", $"DrivingRecords_{userId}_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
