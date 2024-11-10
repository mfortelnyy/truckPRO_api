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
            return View();
        }

        [HttpGet("generate-page")]
        public IActionResult GenerateHtmlPage()
        {
            return View("GeneratedPage"); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}