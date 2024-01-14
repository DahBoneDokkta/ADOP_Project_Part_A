using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;
using Assignment_A1_03.Services;
using System.Xml;

namespace Assignment_A1_03
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            //Register the event
            //Your Code
            service.WeatherForecastAvailable += Service_WeatherForecastAvailable;

            Task<Forecast>[] tasks = { null, null, null, null };
            Exception exception = null;
            try
            {
                // Latitude and longitude for Gävle
                double latitude = 60.67452;
                double longitude = 17.14174;

                //// Get forecast for Gävle based on geolocation
                //Forecast forecastGävle = await service.GetForecastAsync(forecastGävle);

                //// Get forecast for Brisbane based on city
                //Forecast forecastBrisbane = await service.GetForecastAsync(forecastBrisbane.City);

                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Gävle");

                Task.WaitAll(tasks[0], tasks[1]);

                tasks[2] = service.GetForecastAsync(latitude, longitude);
                tasks[3] = service.GetForecastAsync("Gävle");

                //Wait and confirm we get an event showing cached data avaialable
                Task.WaitAll(tasks[2], tasks[3]);
                await Task.WhenAll(tasks[2], tasks[3]);

                //var groupByDate = forecastGävle.Items.GroupBy(x => x.DateTime.DayOfYear)

            }
            catch (Exception ex)
            {
                exception = ex;
                //How to handle an exception
                //Your Code
                Console.WriteLine("Weather service error:");
                Console.WriteLine($"Exception occured: {ex.Message}");
            }

            foreach (var task in tasks)
            {
                //How to deal with successful and fault tasks
                //Your Code

                if (task.IsCompletedSuccessfully)
                {

                    Console.WriteLine("----------------------------------------------------");
                    var currentForecast = task.Result;
                    Console.WriteLine($"\nWeather forecast for {currentForecast.City}\n");

                    var groupByGävle = currentForecast.Items.GroupBy(x => x.DateTime.DayOfYear);
                    foreach (var currentDateGroup in groupByGävle)
                    {
                        DateTime forecastDates = DateTime.Now.AddDays(currentDateGroup.Key - 1);
                        Console.WriteLine(forecastDates.ToString("yyyy-MM-dd"));
                        foreach (var date in currentDateGroup)
                        {
                            Console.WriteLine($"- {date.DateTime.ToString("H:mm")}: {date.Description}, temperature {date.Temperature} Celsius, wind: {date.WindSpeed} m/s.");
                        }
                    }
                }
                else if (task.IsFaulted)
                {
                    Console.WriteLine($"Task faulted: {task.Exception?.InnerException?.Message}");
                }

            }
        }

        //Event handler declaration
        //Your Code
        static void Service_WeatherForecastAvailable(object sender, string message)
        {
            Console.WriteLine($"Event message from weather service: {message} available");
        }
        
    }
}
