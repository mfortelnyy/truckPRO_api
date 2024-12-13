using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph.Education.Classes.Item.Assignments.Item.Submissions.Item.Return;
using Newtonsoft.Json;
using System.Security.Claims;
using truckapi.DTOs;
using truckapi.Models;
using truckPro_api.Hubs;
using truckPRO_api.DTOs;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    [ApiController]
    public class LogEntryController(S3Service s3Service, ILogEntryService logEntryService, IUserService userService, IHubContext<LogHub> hubContext, IFirebaseService firebaseService) : ControllerBase
    {
        private readonly S3Service _s3Service = s3Service;
        private readonly ILogEntryService _logEntryService = logEntryService;
        private readonly IHubContext<LogHub> _hubContext = hubContext;
        private readonly IFirebaseService _firebaseService = firebaseService;
        private readonly IUserService _userService = userService;

        //test endpoint
        [HttpPost]
        [Route("testFire")]
         public async Task<IActionResult> TestFire()
        {
            string? companyId = User.FindFirst("companyId")?.Value;

            if (string.IsNullOrEmpty(companyId))
            {
                return Conflict("Company ID not found in the token.");
            }
            int companyIdInt = int.Parse(companyId);
            await _hubContext.Clients.Group(companyId.ToString()).SendAsync("ReceiveNotification", $"TestFire");

            return Ok($"sent succ!");


        }


        [HttpPost]
        [Route("TestPush")]
        public async Task<IActionResult> TestPush()
        {
            await _userService.GetManagerFcmTokensAsync(1);
            await _hubContext.Clients.Group("1").SendAsync("ReceiveNotification", "Test Push Notification!");
            
            var sent = await _firebaseService.SendTestPushToManagers();
            if (sent)
            {
                return Ok("sent succ!");
            }
            return Conflict("failed to send");

        }


        //test endpoint
        [HttpPost]
        [Route("uploadProfilePic")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile image)
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            String imageUrl = await _s3Service.UploadFileAsync(image, 0);
            if (imageUrl == null)
            {
                return Conflict("Api failed");
            }
            else
            {
                return Ok(imageUrl);
            }
        }



        [HttpPost]
        [Route("uploadPhotos")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UploadPhotos([FromForm] List<IFormFile> images)
        {
           var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
           
            List<string> imageUrls = await _s3Service.UploadPhotos(images, []);
            if (imageUrls.Count == 0)
            {
                return Conflict("Api failed");
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
                Console.WriteLine("userid:  ", userId);

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
                if(result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict(result);
                }
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
        public async Task<IActionResult> CreateDrivingLog([FromForm] List<IFormFile> images, [FromForm] string promptImages)
        {
            string? companyId = User.FindFirst("companyId")?.Value;

            if (string.IsNullOrEmpty(companyId))
            {
                return Conflict("Company ID not found in the token.");
            }
            int companyIdInt = int.Parse(companyId);

            try
            {
                string res = "";
                var userId = User.FindFirst("userId").Value;
                //Deserialize the promptImages JSON string into a List of PromptImage objects
                var promptImagesList = JsonConvert.DeserializeObject<List<PromptImage>>(promptImages);
                Console.WriteLine(promptImages);
                //Upload the images and get their URLs
                List<string> imageUrls = await _s3Service.UploadPhotos(images, promptImagesList);
                foreach (var item in imageUrls)
                {
                    Console.WriteLine(item);
                }

                if (imageUrls.Count == 0)
                {
                    return Conflict("Image upload failed");
                }

                //Validate and process the driving log entry

                LogEntry logEntry = new()
                {
                    UserId = int.Parse(userId),
                    StartTime = DateTime.Now,
                    EndTime = null,
                    LogEntryType = LogEntryType.Driving,
                    ImageUrls = imageUrls,

                };

                res = await _logEntryService.CreateDrivingLog(logEntry);
                
                await _hubContext.Clients.Group(companyId.ToString()).SendAsync("ReceiveNotification", $"Log by user id {logEntry.UserId} at {logEntry.StartTime}");
                
                UserDTO user = await _userService.GetUserById(int.Parse(userId));
                var sent = await _firebaseService.SendDriverDrivingPushToManagers(companyIdInt, "Approve Log!", user.FirstName, user.LastName);

                // Return success with the uploaded URLs
                if(res.Contains("successfully"))
                {
                    return Ok($"{res} Managers notified: {sent}");
                }
                else
                {
                    return Conflict(res);
                }                
            

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }

            catch (Exception ex)
            {
                // Log the error and return a bad request
                return BadRequest(new { message = ex.Message });
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
                if(res.Contains("successfully"))
                {
                    return Ok(res);
                }
                else
                {
                    return Conflict(res);
                }
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
        [Route("createBreakLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateBreakLog()
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
                    LogEntryType = LogEntryType.Break,
                    ImageUrls = null,
                };

                var result = await _logEntryService.CreateBreakLog(logEntry);
                if(result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict(result);
                }
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
                if(result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict(result);
                }

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
                if(result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict(result);
                }
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
                if(result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict(result);
                }

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
        [Route("stopBreakLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StopBreakLog()
        {
            try
            {
                var userId = User.FindFirst("userId").Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.StopBreakLog(int.Parse(userId));
                if(result.Contains("successfully"))
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict(result);
                }

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
    
        
        
        [HttpGet]
        [Route("getTotalDrivingHoursLastWeek")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetTotalDrivingHoursLastWeek()
        {
            try
            {
                var driverId =  User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
                if (string.IsNullOrEmpty(driverId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.GetTotalDrivingHoursLastWeek(int.Parse(driverId));
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


        [HttpGet]
        [Route("getTotalOnDutyHoursLastWeek")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetTotalOnDutyHoursLastWeek()
        {
            try
            {
                var driverId =  User.Claims.FirstOrDefault(c => c.Type == "userId").Value;

                if (string.IsNullOrEmpty(driverId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.GetTotalOnDutyHoursLastWeek7days(int.Parse(driverId));
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


        [HttpGet]
        [Route("getTotalOffDutyHoursLastWeek")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetTotalOffDutyHoursLastWeek()
        {
            try
            {
                var driverId =  User.Claims.FirstOrDefault(c => c.Type == "userId").Value;

                if (string.IsNullOrEmpty(driverId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.GetTotalOffDutyHoursLastWeek(int.Parse(driverId));
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

        [HttpGet]
        [Route("getActiveLogs")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetActiveLogs()
        {
            try
            {
                var driverId = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;

                if (string.IsNullOrEmpty(driverId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.GetActiveLogEntries(int.Parse(driverId));
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

        [HttpGet]
        [Route("getAllLogs")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var driverId = User.Claims.FirstOrDefault(c => c.Type == "userId").Value;

                if (string.IsNullOrEmpty(driverId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                var result = await _logEntryService.GetAllLogs(int.Parse(driverId));
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
