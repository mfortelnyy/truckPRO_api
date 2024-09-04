using Microsoft.AspNetCore.Mvc;

namespace truckPRO_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(ILogger<RegistrationController> logger)
        {
            _logger = logger;
        }

        [Route("/signIn")]
        [HttpGet]
        public string SignIn(string userName,string password)
        {
            return $"signin in with uName: {userName} and pswd: {password}";
        }

        [Route("/register")]
        [HttpPost]
        public string Register(string userName, string password)
        {
            return $"sign up with uName: {userName} and pswd: {password}";
        }
        /*
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        */
    }
}
