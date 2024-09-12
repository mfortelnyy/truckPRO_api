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
            try
            {
                //Get userid form token
                var userId = User.FindFirst("userId").Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }


                LogEntry logEntry = new()
                {
                    UserId = int.Parse(userId),
                    StartTime = DateTime.Now,
                    EndTime = null,
                    LogEntryType = LogEntryType.OnDuty,
                    ImageUrls = null,

                };
                var result = await _logEntryService.CreateOnDutyLog(logEntry);


                //Console.WriteLine(userId);
                return Ok($"OnDutyLogEntry with id {result} was added");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }







        [HttpPost]
        [Route("createDrivingLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateDrivingLog([FromForm] IFormFileCollection images)
        {
            try
            {
                var userId = User.FindFirst("userId").Value;


                //Console.WriteLine($"userId :   {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                List<string> imageUrls = await _s3Service.UploadPhotos(images);
                if (imageUrls.Count < 0)
                {
                    return NotFound("Image upload failed");
                }

                // Validate and process the driving log entry

                LogEntry logEntry = new()
                {
                    UserId = int.Parse(userId),
                    StartTime = DateTime.Now,
                    EndTime = null,
                    LogEntryType = LogEntryType.Driving,
                    ImageUrls = imageUrls,

                };

                var res = await _logEntryService.CreateDrivingLog(logEntry);
                return Ok($"DrivingLogEntry with id {res} was added");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [Route("createOffDutyLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateOffDutyLog()
        {
            try
            {
                var userId = User.FindFirst("userId").Value;


                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                LogEntry logEntry = new()
                {
                    UserId = int.Parse(userId),
                    StartTime = DateTime.Now,
                    EndTime = null,
                    LogEntryType = LogEntryType.OffDuty,
                    ImageUrls = null,
                };

                var res = await _logEntryService.CreateOffDutyLog(logEntry);
                return Ok($"Off Duty log with id {res} was added");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [Route("stopDrivingLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StopDrivingLog()
        {
            try
            {
                var userId = User.FindFirst("userId").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.StopDrivingLog(int.Parse(userId));
                return Ok(result);

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("stopOnDutyLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StopOnDutyLog()
        {
            try
            {
                var userId = User.FindFirst("userId").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.StopOnDutyLog(int.Parse(userId));
                return Ok(result);

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("stopOffDutyLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StopOffDutyLog()
        {
            try
            {
                var userId = User.FindFirst("userId").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.StopOffDutyLog(int.Parse(userId));
                return Ok(result);

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
