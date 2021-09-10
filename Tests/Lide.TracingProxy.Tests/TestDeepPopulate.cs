using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lide.TracingProxy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.TracingProxy.Tests
{
    [TestClass]
    public class TestDeepPopulate
    {

        [TestMethod]
        public void TryPopulate()
        {
            string a = null;
            Console.WriteLine(a.GetType());
        }
        
        

        public class Test
        {
            public string Data { get; }

            public Test(string data, Test d)
            {
                Data = data;
            }
        }
        
        
        
    }
}