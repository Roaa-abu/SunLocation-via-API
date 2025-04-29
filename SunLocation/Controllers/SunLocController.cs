using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SunLocation.Controllers
{
    public class SunLocController : Controller
    {
        private readonly HttpClient _httpServe;
        public SunLocController(HttpClient httpServe)
        {
            _httpServe = httpServe;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SunLoc()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetSunData(string Latitude, string Longitude)
        {
            string response = string.Empty;
            if (Latitude != null && Longitude != null)
            {
                var url = $"https://api.timezonedb.com/v2.1/get-time-zone?key=9O0I0SD7BUDW&format=json&by=position&lat={Latitude}&lng={Longitude}";
                response = _httpServe.GetStringAsync(url).Result;
            }
            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;

            string? status = root.GetProperty("status").GetString();
            string? formatted = root.GetProperty("formatted").GetString();
            string sunSrc = "";
            if (formatted != null)
            {
                string[] parts = formatted.Split(' ');
                string date = parts[0];
                string[] time = parts[1].Split(':');
                int hour = int.Parse(time[0]);
                int minute = int.Parse(time[1]);
                int second = int.Parse(time[2]);
                if (hour == 5 && minute >= 50)
                    sunSrc = "/img/sunrise.png";
                else if (hour == 19 && minute >= 10)
                    sunSrc = "/img/sunset.png";
                else if (hour >= 6 && hour <= 11)
                    sunSrc = "/img/morning.png";
                else if (hour >= 12 && hour <= 14)
                    sunSrc = "/img/noon.png";
                else if (hour >= 15 && hour <= 18)
                    sunSrc = "/img/evening.png";
                else
                   sunSrc = "/img/night.png"; // default
            }
        
            else
            {
                View("SunLoc");
            }
            ViewBag.SunImagePath = sunSrc;
            return View("ViewResponse");
             //          ViewResponse
        }
        [HttpGet]
        public IActionResult ViewResponse()
        {
            ViewBag.SunImagePath = "/img/night.png";
            return View();
        }
    }
}
