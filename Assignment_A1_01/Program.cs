using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // For Österåkers kommun
            //double latitude = 59.5086798659495;
            //double longitude = 18.2654625932976;

            // Latitude and longitude for Gävle, Sörby (Closest weatherstation?)
            double latitude = 60.67452;
            double longitude = 17.14174;

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

            //Your Code to present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");

            var groupByDate = forecast.Items.GroupBy(x => x.DateTime.DayOfYear);

            foreach (var item in groupByDate)
            {
                DateTime forecastDates = new DateTime(DateTime.Now.Year, 1, 1).AddDays(item.Key - 1);

                Console.WriteLine(forecastDates.ToString("yyyy-MM-dd"));
                foreach (var date in item)
                {
                    Console.WriteLine($" - {date.DateTime.ToString("H:mm")}: {date.Description}, temperature {date.Temperature} Celsius, wind: {date.WindSpeed} m/s"); 
                }
            }
        }
    }
}
