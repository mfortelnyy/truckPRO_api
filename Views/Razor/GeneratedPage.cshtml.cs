using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace truckapi.Views.Razor
{
    public class GeneratedPage : PageModel
    {
        private readonly ILogger<GeneratedPage> _logger;

        public GeneratedPage(ILogger<GeneratedPage> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}