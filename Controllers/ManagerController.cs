using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    public class ManagerController(IManagerService managerService): Controller
    {
        [HttpGet]
        [Route("/adm/addDriverByCompanyId")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddDriverByCompanyId(int companyId)
        {
            var res = await managerService.AddDriverByCompanyId(companyId);
            return Ok(res);
        }
    }
}
