﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;

namespace Assignment_A1_01.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "073779edd27cdf4d54d4616b0ffc20b2"; 

        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            //Convert WeatherApiData to Forecast using Linq.
            //Your code
            var forecast = new Forecast();
            forecast.City = wd.city.name;
            //forecast.Items = new List<ForecastItem>();


            forecast.Items = wd.list.Select(x => new ForecastItem()
            {
                DateTime = UnixTimeStampToDateTime(x.dt),
                Temperature = x.main.temp,
                WindSpeed = x.wind.speed,
                Description = x.weather[0].description,
                Icon = $"https://openweathermap.org/img/w/{x.weather.First().icon}.png"

            }).ToList();

            //Hint: you will find 
            //City: wd.city.name
            //Daily forecast in wd.list, in an item in the list
            //      Date and time in Unix timestamp: dt 
            //      Temperature: main.temp
            //      WindSpeed: wind.speed
            //      Description:  first item in weather[].description
            //      Icon:  $"http://openweathermap.org/img/w/{wdle.weather.First().icon}.png"   //NOTE: Not necessary, only if you like to use an icon

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
