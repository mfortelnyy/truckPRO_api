using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using truckPRO_api.Models;
using truckPRO_api.Services;

namespace truckPRO_api.Controllers
{
    [ApiController]
    public class LogEntryController(S3Service s3Service) : ControllerBase
    {
        private readonly S3Service _s3Service = s3Service;


        [HttpPost]
        [Route("createOnDuty")]
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
    }
}
