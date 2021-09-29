using System;
using System.Linq;
using System.Text;
using Lide.Core.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Core.Tests
{
    [TestClass]
    public class TestMethodDependencies
    {
        [TestMethod]
        public void That_ListWithAllDependencyTypesIsProduced()
        {
            var methodInfo = typeof(Tester).GetMethods().FirstOrDefault(x => x.Name == "Method");
            var provider = new MethodDependencies();
            
            var dependencies = provider.GetDependencies(methodInfo);
            Assert.AreEqual(3, dependencies.Count);
            CollectionAssert.Contains(dependencies, typeof(Console));
            CollectionAssert.Contains(dependencies, typeof(Exception));
            CollectionAssert.Contains(dependencies, typeof(Encoding));
        }
        
        [TestMethod]
        public void That_ListWithAllDependencies_When_DelegateIsUsed()
        {
            var methodInfo = typeof(TesterBroken).GetMethods().FirstOrDefault(x => x.Name == "Method");
            var provider = new MethodDependencies();
            
            var dependencies = provider.GetDependencies(methodInfo).Where(x => x != null).ToList();
            Assert.AreEqual(1, dependencies.Count);
            CollectionAssert.Contains(dependencies, typeof(System.Action<string>));
        }

        private class TesterBroken
        {
            public void Method()
            {
                Action<string> b = Console.WriteLine;
                b("Something");
            }
        }

        private class Tester
        {
            public void Method()
            {
                var e = new Exception();
                var c = Encoding.UTF8.GetBytes("Data");
                Internal();
            }

            private static void Internal()
            {
                Console.WriteLine("Something");
            }

        }
    }
}