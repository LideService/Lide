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
        private readonly IKarta _t;
        private readonly IKarta2 _t2;

        public WeatherForecastController(IKarta t, IKarta2 t2)
        {
            Console.WriteLine("ctor");
            _t = t;
            _t2 = t2;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _t.Do();
            _t2.Do();
            return Enumerable.Empty<string>();
        }
    }

    public interface IKarta
    {
        void Do();
    }
    public interface IKarta2
    {
        void Do();
    }

    public class Obb : IKarta
    {
        public void Do()
        {
            Console.WriteLine("Obb");
        }
    }
    
    public class Unc : IKarta
    {
        public void Do()
        {
            Console.WriteLine("Unc");
        }
    }

    
    public class Dsk : IKarta2
    {
        private static readonly Random R = new Random();
        private readonly int _id;
        public Dsk()
        {
            _id = R.Next();
        }
        public void Do()
        {
            Console.WriteLine($"Dsk {_id}");
        }
    }
    
}