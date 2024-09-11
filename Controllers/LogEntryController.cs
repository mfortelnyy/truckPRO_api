using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using System.Security.Claims;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    [ApiController]
    public class LogEntryController(S3Service s3Service, ILogEntryService logEntryService) : ControllerBase
    {
        private readonly S3Service _s3Service = s3Service;
        private readonly ILogEntryService _logEntryService = logEntryService;


        
        //test endpoint
        [HttpPost]
        [Route("uploadPhotos")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UploadPhotos([FromForm] IFormFileCollection images)
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
        [Route("createOnDutyLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateOnDutyLog()
        {
            //Get userid form token
            var userId = User.FindFirst("userId").Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }


            LogEntry logEntry = new LogEntry
            {
                UserId = int.Parse(userId),
                StartTime = DateTime.Now,
                EndTime = null,
                LogEntryType = LogEntryType.OnDuty,
                ImageUrls = null,

            };
             var result = await _logEntryService.CreateOnDutyLog(logEntry);


            Console.WriteLine(userId);
            return Ok();
        }







        [HttpPost]
        [Route("createDrivingLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateDrivingLog([FromForm] IFormFileCollection images)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in the token.");
            }

            List<string> imageUrls = await _s3Service.UploadPhotos(images);
            if (imageUrls.Count == 0)
            {
                return NotFound("Image upload failed");
            }

            // Validate and process the driving log entry
            Console.WriteLine(userId);
            //var res = _logEntryService.CreateDrivingLog();
            return Ok();
        }
    }
}
