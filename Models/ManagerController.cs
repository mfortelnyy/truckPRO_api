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
        [Route("AllDriversByCompany")]
        public async Task<IActionResult> GetAllDriversByCompany()
        {
            try
            {
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var companyId = int.Parse(User.FindFirst("companyId").Value);
                var drivers = await _managerService.GetAllDriversByCompany(companyId);
                if (drivers == null || drivers.Count == 0)
                {
                    return NoContent();
                }
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
