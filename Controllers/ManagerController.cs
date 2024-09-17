using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    public class ManagerController(IManagerService managerService, IEmailService emailService): Controller
    {
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
                var pendingDrivers = await managerService.GetPendingUsersByCompanyId(int.Parse(companyId));
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


        
    }
}
