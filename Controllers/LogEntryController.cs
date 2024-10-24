﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using truckapi.Models;
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
        [Route("uploadPhoto")]
        [Authorize(Roles = "Driver")]
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







  /*      [HttpPost]
        [Route("createDrivingLog")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateDrivingLog([FromForm] List<IFormFile> images, [FromForm] String promptImages)
        {
            string res = "";
            try
            {
                var imageInfoList = JsonConvert.DeserializeObject<List<PromptImage>>(promptImages) ?? throw new Exception("Wrong format!");
       
                var userId = User.FindFirst("userId").Value;
                

                //Console.WriteLine($"userId :   {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in the token.");
                }

                List<string> imageUrls = await _s3Service.UploadPhotos(images, imageInfoList);
                if (imageUrls.Count == 0)
                {
                    return Conflict("Image upload failed");
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

                res = await _logEntryService.CreateDrivingLog(logEntry);
                
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok($"DrivingLogEntry with id {res} was added");
        }

*/
        [HttpPost]
        [Route("createDrivingLog")]
        [Authorize(Roles = "Driver")]
public async Task<IActionResult> CreateDrivingLog([FromForm] List<IFormFile> images, [FromForm] string promptImages)
{
    try
    {
        string res = "";
        var userId = User.FindFirst("userId").Value;
        // Deserialize the promptImages JSON string into a List of PromptImage objects
        var promptImagesList = JsonConvert.DeserializeObject<List<PromptImage>>(promptImages);
        Console.WriteLine(promptImages);
        // Upload the images and get their URLs
        List<string> imageUrls = await _s3Service.UploadPhotos(images, promptImagesList);
        foreach (var item in imageUrls)
        {
            Console.WriteLine(item);
        }
        if (imageUrls.Count == 0)
        {
            return Conflict("Image upload failed");
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

        res = await _logEntryService.CreateDrivingLog(logEntry);
        // Return success with the uploaded URLs
        return Ok($"Off Duty log with id {res} was added");
        
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


        [HttpGet]
        [Route("getActive")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetActiveLogEntries()
        {
            try
            {
                var driverId =  User.Claims.FirstOrDefault(c => c.Type == "userId").Value;
       
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

                var result = await _logEntryService.GetTotalOnDutyHoursLastWeek(int.Parse(driverId));
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
