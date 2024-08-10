using AzRedisRepository.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AzRedisRepository.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IRedisHashRepository<WeatherForecast> redisHashRepository) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
         "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
     };

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await redisHashRepository.RemoveAllAsync("weatherforecast:*");

            var wCast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
            await redisHashRepository.StoreAsync($"weatherforecast:1", wCast);

            wCast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(2),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            await redisHashRepository.StoreAsync($"weatherforecast:2", wCast);

            wCast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(3),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            await redisHashRepository.StoreAsync($"weatherforecast:3", wCast);

            await redisHashRepository.RemoveHashAsync($"weatherforecast:2");

            var allForecasts = await redisHashRepository.GetAllAsync("weatherforecast:*");
            foreach (var item in allForecasts)
            {
                Console.WriteLine($"Date: {item.Date}, TemperatureC: {item.TemperatureC}, Summary: {item.Summary}");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }

}
