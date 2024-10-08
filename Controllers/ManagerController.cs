﻿using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    public class ManagerController(IManagerService managerService, IAdminService adminService, IEmailService emailService, S3Service s3Service): Controller
    {
        private readonly S3Service _s3Service = s3Service;
   
        [HttpPost]
        [Route("addDriversToCompany")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddDriversByCompanyId(IFormFile file )
        {
            var companyId = User.Claims.FirstOrDefault(c => c.Type == "companyId");
       
            if (file == null || file.Length == 0)
            {
                return BadRequest("No File Uploaded!");
            }
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase))
            {
                return BadRequest("Invalid file type. Please upload an Excel file.");

            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var drivers = new List<PendingUser>();

                    using (var package = new ExcelPackage(stream))
                    {
                        //get the first worksheet
                        var worksheet = package.Workbook.Worksheets[0];

                        //skips the header row
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var email = worksheet.Cells[row, 1].Text;
                            Console.WriteLine(email);

                            if (!string.IsNullOrEmpty(email))
                            {
                                PendingUser newPendingUser = new()
                                {
                                    Email = email,
                                    CompanyId = int.Parse(companyId.Value),
                                    CreatedDate = DateTime.Now,
                                    InvitationSent = false,

                                };
                                await managerService.AddDriverToCompany(newPendingUser);
                            }
                        }
                    }

                }

                return Ok("File processed and drivers added successfully!");
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
        [Route("sendEmailToPendingUsers")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> SendEmailToPendingUsers()
        {
            try
            {
                string companyId = User.Claims.FirstOrDefault(c => c.Type == "companyId").Value
                    ?? throw new InvalidOperationException();
                string link = "";
                var pendingDrivers = await managerService.GetPendingDriversByCompanyId(int.Parse(companyId));
                foreach (var driver in pendingDrivers)
                {
                    await emailService.SendEmailAsync(
                        email: driver.Email,
                        subject: "TruckPro Registration",
                        message: $"Dear Driver, Please register with the following email - {driver.Email} to our system. " +
                        $"\nFollow this link - {link}"
                        );
                    //email was sent then change flag to true
                    driver.InvitationSent = true;
                    await managerService.UpdatePendingDriver(driver);

                }
                return Ok("Emails sent successfully!");
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
        [Route("getAllActiveDrivingLogs")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllActiveDrivingLogs()
        {
            try
            {
                var companyId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "companyId").Value);
                var activeDrivingLogs = await managerService.GetAllActiveDrivingLogs(companyId);
                return Ok(activeDrivingLogs);
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
        [Route("approveDrivingLogById")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveDrivingLogById([FromQuery] int logEntryId)
        {
            try
            {
                var res = await managerService.ApproveDrivingLogById(logEntryId);
                return Ok(res);
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
        [Route("getAllDriversByCompany")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GgetAllDriversByCompany()
        {
            var companyId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "companyId").Value);

            try
            {
                var drivers = await adminService.GetDriversByComapnyId(companyId);
                return Ok(drivers);
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
        [Route("geLogsByDriverId")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetLogsByDriverId([FromQuery] int driverId)
        {

            try
            {
                var drivers = await adminService.GetLogsByDriverId(driverId);
                return Ok(drivers);
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
        [Route("getImagesOfDrivingLog")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetImagesOfDrivingLog([FromQuery] int drivingLogId)
        {
            try
            {
                var res = await managerService.GetImagesOfDrivingLog(drivingLogId);
                return Ok(res);

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


        //returns list of users that registered from pendingUsers table
        [HttpGet]
        [Route("getRegisteredFromPending")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetRegisteredFromPending()
        {
            var companyId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "companyId").Value);
            try
            {
                var res = await managerService.GetRegisteredFromPending(companyId);
                return Ok(res);

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


        //returns list of pendingusers that did not register yet from pendingUsers table
        [HttpGet]
        [Route("getNotRegisteredFromPending")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetNotRegisteredFromPending()
        {
            var companyId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "companyId").Value);
            try
            {
                var res = await managerService.GetNotRegisteredFromPending(companyId);
                return Ok(res);

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
        [Authorize(Roles = "Manager")]
        [Route("allDriversByCompany")]
        public async Task<IActionResult> GetAllDriversByCompany()
        {
            try
            {
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                string? cIdString = User.FindFirst("companyId").Value;

                Console.WriteLine("companyid:  "+cIdString);

                if (cIdString != null) 
                {
                    var companyId = int.Parse(cIdString);
                    var drivers = await managerService.GetAllDriversByCompany(companyId);
                    if (drivers == null || drivers.Count == 0)
                    {
                        return NotFound("Sorry, no drivers found in your company!");
                    }
                    return Ok(drivers);
                }
                return BadRequest("company id nor found from token!");
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        [Route("allLogsByDriver")]
        public async Task<IActionResult> GetAllLogsByDriver([FromQuery] int driverId)
        {
            try
            {
                var companyId = int.Parse(User.FindFirst("companyId").Value);
                var allLogs = await managerService.GetLogsByDriver(driverId, companyId);
                if (allLogs == null || allLogs.Count == 00)
                {
                    return NotFound("Sorry, driver has no logs at this moment!");
                }
                return Ok(allLogs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        [Route("getSignedUrls")]
        public async Task<IActionResult> GetSignedUrls([FromBody] List<string> urls)
        {
            try
            {
                var companyId = int.Parse(User.FindFirst("companyId").Value);
                var signedUrls = _s3Service.GenerateSignedUrls(urls);
                if (signedUrls == null || signedUrls.Count == 00)
                {
                    return NotFound("Sorry, no signed urls can be generated!");
                }
                return Ok(signedUrls);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getAllPendingUsers")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> getAllPendingUsers()
        {
            try
            {
                var companyId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "companyId").Value);
                var activeDrivingLogs = await managerService.GetAllActiveDrivingLogs(companyId);
                return Ok(activeDrivingLogs);
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
