using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.WebAPI.Tests
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void Do()
        {
            // var client = new HttpClient();
            // var handlerInfo = typeof(HttpMessageInvoker).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).First(x => x.Name == "_handler");
            // var handler = (HttpMessageHandler)handlerInfo.GetValue(client);
            //
            // var newHandler = Activator.CreateInstance(handler.GetType());
            // handlerInfo.SetValue(client, newHandler);

            var methodInfo = typeof(A).GetMethods().First(x => x.Name == "Do");
            var obj = new B();

            methodInfo.Invoke(obj, null);

        }


        public class A
        {
            public virtual void Do()
            {
                Console.Write("A");
            }
        }

        public class B : A
        {
            public override void Do()
            {
                Console.Write("B");
            }
        }
    }
}