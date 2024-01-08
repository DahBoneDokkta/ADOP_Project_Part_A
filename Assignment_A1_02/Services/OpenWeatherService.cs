using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_02.Models;

namespace Assignment_A1_02.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "073779edd27cdf4d54d4616b0ffc20b2"; 

        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        protected virtual void OnWeatherForecastAvailable (string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            Forecast forecast = await ReadWebApiAsync(uri);

            //Event code here to fire the event
            //Your code

            //var forecast = new Forecast();
            forecast.City = wd.city.name;
            //forecast.Items = new List<ForecastItem>();

            //Convert WeatherApiData to Forecast using Linq.
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
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //Event code here to fire the event
            //Your code


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
