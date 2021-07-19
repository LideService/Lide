using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lide.WebApiTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public WeatherForecastController(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            var cl1 =httpClientFactory.CreateClient();
            cl1.DefaultRequestHeaders.Add("T", "2");
            var cl2 = httpClientFactory.CreateClient();
            
            Console.WriteLine("ctor");
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Enumerable.Empty<string>();
        }

        
        [HttpPost]
        [Route("/get2")]
        public IEnumerable<string> Get2()
        {;
            return Enumerable.Empty<string>();
        }
    }
}