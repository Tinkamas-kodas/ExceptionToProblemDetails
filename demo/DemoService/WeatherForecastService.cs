using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoService
{
    public class WeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private static Random Random = new Random();
        private static int cnt = 0;
        public IEnumerable<WeatherForecast> GetForecastForLocation(string location)
        {
            if (!location.Equals("Vilnius", StringComparison.CurrentCultureIgnoreCase))
                throw new NotFoundException();

            cnt++;
            if (cnt % 2 == 0)
                throw new Exception("Server error");
            

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Next(-20, 55),
                Summary = Summaries[Random.Next(Summaries.Length)]
            });
        }
    }
}
