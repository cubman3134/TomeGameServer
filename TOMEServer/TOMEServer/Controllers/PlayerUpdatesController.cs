using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace TOMEServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerUpdatesController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public PlayerUpdatesController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPlayerUpdates")]
        public WeatherForecast Get()
        {
            return new WeatherForecast();
        }
    }
}