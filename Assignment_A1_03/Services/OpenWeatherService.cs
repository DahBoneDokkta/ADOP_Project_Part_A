using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        
        //Cache declaration
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        // My API Key
        readonly string apiKey = "073779edd27cdf4d54d4616b0ffc20b2";

        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        protected virtual void OnWeatherForecastAvailable (string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code
            if (cachedCityForecasts.ContainsKey((City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                WeatherForecastAvailable?.Invoke(this, $"Cached weather forecast for {City}");
                return cachedCityForecasts[(City, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];
            }
            

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code
            cachedCityForecasts.AddOrUpdate(
                (City, DateTime.Now.ToString("yyyy-MM-dd HH:mm")), forecast, (key, existingForecast) => existingForecast);
            WeatherForecastAvailable?.Invoke(this, $"New weather for {City}");

            Console.WriteLine($"Event message from weather service: New weather forecast for {City} available");

            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code
            if (cachedGeoForecasts.ContainsKey((latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))))
            {
                WeatherForecastAvailable?.Invoke(this, $"Cached weather forecast for {latitude} {longitude}");
                return cachedGeoForecasts[(latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))];
            }

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code
            cachedGeoForecasts.AddOrUpdate(
            (latitude, longitude, DateTime.Now.ToString("yyyy-MM-dd HH:mm")), forecast, (key, existingForecast) => existingForecast);
            WeatherForecastAvailable.Invoke(this, $"New weather for {latitude} {longitude} available.");

            //Console.WriteLine($"Event message from weather service: {forecast.City} available");
            //Console.WriteLine($"Event message from weather service: New weather forecast for {latitude}, {longitude}) available");

            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();


            //Convert WeatherApiData to Forecast using Linq.
            //Your code
            var forecast = new Forecast();
            forecast.City = wd.city.name;
            forecast.Items = wd.list.Select(x => new ForecastItem()
            {
                DateTime = UnixTimeStampToDateTime(x.dt),
                Temperature = x.main.temp,
                WindSpeed = x.wind.speed,
                Description = x.weather[0].description,
                Icon = $"https://openweathermap.org/img/w/{x.weather.First().icon}.png"
            }).ToList();
            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
