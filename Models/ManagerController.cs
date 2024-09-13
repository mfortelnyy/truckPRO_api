using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truckPRO_api.Services;

namespace truckPRO_api.Models
{

    [ApiController]
    public class ManagerController(IManagerService _managerService) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Manager")]
        [Route("allDriversByCompany")]
        public async Task<IActionResult> GetAllDriversByCompany()
        {
            try
            {
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var companyId = int.Parse(User.FindFirst("companyId").Value);
                var drivers = await _managerService.GetAllDriversByCompany(companyId);
                if (drivers == null || drivers.Count == 0)
                {
                    return NotFound("Sorry, no drivers found in your company!");
                }
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        [Route("allLogsByDriver")]
        public async Task<IActionResult> GetAllLogsByDriver(int driverId)
        {
            try
            {
                var companyId = int.Parse(User.FindFirst("companyId").Value);
                var allLogs = await _managerService.GetLogsByDriver(driverId, companyId);
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
    }
}
