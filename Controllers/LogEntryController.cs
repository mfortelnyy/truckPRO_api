using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    [ApiController]
    public class LogEntryController(S3Service s3Service, LogEntryService logEntryService) : ControllerBase
    {
        private readonly S3Service _s3Service = s3Service;
        private readonly LogEntryService _logEntryService = logEntryService;


        [HttpPost]
        [Route("createOnDutyLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateOnDuty([FromForm] IFormFileCollection images)
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            Console.WriteLine(token);
            List<string> imageUrls = await _s3Service.UploadPhotos(images);
            if (imageUrls.Count == 0)
            {
                return NotFound("Api failed");
            }
            else
            {
                return Ok(imageUrls);
            }   
        }

        [HttpPost]
        [Route("createDrivingLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateDrivingLog([FromBody] DrivingLogEntryDTO drivingLogEntryDTO)
        {
            // Validate and process the driving log entry
            var result = await ;
            return Ok(result);
        }
    }
}
