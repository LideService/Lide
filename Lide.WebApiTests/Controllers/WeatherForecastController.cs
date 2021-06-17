using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Lide.WebApiTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static IT last;
        private readonly IT _t;
        private readonly IServiceProvider _serviceProvider;

        public WeatherForecastController(IT t, IServiceProvider serviceProvider)
        {
            Console.WriteLine("ctor");
            _t = t;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            if (last != null)
            {
                Console.WriteLine(last.Equals(_t));
            }
            _t.Do();
            last = _t;
            
            return Enumerable.Empty<string>();
        }
    }

    public interface IT
    {
        void Do();
    }
    public interface IT2
    {
        void Do();
    }

    public class T : IT
    {
        private readonly IT2 _it2;

        public T(IT2 it2)
        {
            _it2 = it2;
        }
        public void Do()
        {
            _it2.Do();
            Console.WriteLine("Do1");
        }
    }
    
    public class T2 : IT2
    {
        public void Do()
        {
            Console.WriteLine("Do2");
        }
    }
    
}