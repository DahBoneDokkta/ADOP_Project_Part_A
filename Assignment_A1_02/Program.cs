using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Assignment_A1_02.Models;
using Assignment_A1_02.Services;

namespace Assignment_A1_02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            // Register the event
            service.WeatherForecastAvailable += Service_WeatherForecastAvailable;

            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {
                double latitude = 60.67452;
                double longitude = 17.14174;

                // Get forecast for Gävle based on geolocation
                Forecast forecastGävle = await service.GetForecastAsync(latitude, longitude);

                // Present Gävle forecast details
                Console.WriteLine($"Weather forecast for {forecastGävle.City}");
                var groupByDateGävle = forecastGävle.Items.GroupBy(x => x.DateTime.DayOfYear);
                foreach (var item in groupByDateGävle)
                {
                    DateTime forecastDates = new DateTime(DateTime.Now.Year, 1, 1).AddDays(item.Key - 1);
                    Console.WriteLine(forecastDates.ToString("yyyy-MM-dd"));
                    foreach (var date in item)
                    {
                        Console.WriteLine($" - {date.DateTime.ToString("H:mm")}: {date.Description}, temperature {date.Temperature} Celsius, wind: {date.WindSpeed} m/s");
                    }
                }

                // Get forecast for Tokyo
                tasks[0] = service.GetForecastAsync(latitude, longitude); // <-- Use the coordinates for Tokyo
                tasks[1] = service.GetForecastAsync("Tokyo");

                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var currentForecast = await task;
                        Console.WriteLine($"Weather forecast for {currentForecast.City}");

                        var groupByDateTokyo = currentForecast.Items.GroupBy(x => x.DateTime.DayOfYear);
                        foreach (var currentDateGroup in groupByDateTokyo)
                        {
                            DateTime forecastDates = DateTime.Now.AddDays(currentDateGroup.Key - 1);
                            Console.WriteLine(forecastDates.ToString("yyyy-MM-dd"));
                            foreach (var date in currentDateGroup)
                            {
                                Console.WriteLine($" - {date.DateTime.ToString("H:mm")}: {date.Description}, temperature {date.Temperature} Celsius, wind: {date.WindSpeed} m/s");
                            }
                        }
                    }
                    else if (task.IsFaulted)
                    {
                        Console.WriteLine($"Task faulted: {task.Exception?.InnerException?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            foreach (var task in tasks)
            {
                if (task != null)
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine($"Task faulted: {task.Exception?.InnerException?.Message}");
                    }
                    else if (task.IsCompletedSuccessfully)
                    {
                        var forecast = await task;
                        Console.WriteLine($"Weather forecast for {forecast.City}");
                    }
                }
            }
        }

        // Event handler declaration
        private static void Service_WeatherForecastAvailable(object sender, string e)
        {
            Console.WriteLine($"Event received: {e}");
        }
    }
}
