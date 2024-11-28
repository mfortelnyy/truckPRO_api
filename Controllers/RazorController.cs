using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace truckapi.Controllers
{
    public class RazorController : Controller
    {
        private readonly ILogger<RazorController> _logger;

        public RazorController(ILogger<RazorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToPage("/Register");
        }

        [HttpGet("verify-email")]
        public IActionResult GenerateHtmlPage()
        {
            return View("VerifyEmail"); 
        }

        [HttpGet("PrivacyPolicy")]
        public IActionResult PrivacyPolicy()
        {
            return View("PrivacyPolicy");
        }


        // [HttpGet("signup-page")]
        // public IActionResult SignUpPage()
        // {
        //     return View("Register");
        // }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}