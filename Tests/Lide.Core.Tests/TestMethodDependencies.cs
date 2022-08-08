using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Lide.AsyncProxy;
using Lide.Core.Provider;
using Lide.Core.Tests.Proxy;
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
            
            var provider = new MethodDependenciesProvider();

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
            var provider = new MethodDependenciesProvider();
            
            var dependencies = provider.GetDependencies(methodInfo).Where(x => x != null).ToList();
            Assert.AreEqual(3, dependencies.Count);
            CollectionAssert.Contains(dependencies, typeof(System.Action<object>));
            CollectionAssert.Contains(dependencies, typeof(File));
        }
        
        [TestMethod]
        public void That_TesterWithDecoratedDependency_FindsTheDecorator()
        {
            var methodInfo = typeof(TesterWithDecoratedDependency).GetMethods().FirstOrDefault(x => x.Name == "Execute");
            var provider = new MethodDependenciesProvider();
            var proxy = DispatchProxyAsyncFactory.Create(typeof(ITesterDependency), typeof(BaseObjectProxy));
            ((BaseObjectProxy)proxy).BaseObject = new TesterDependency();
            var testObject = new TesterWithDecoratedDependency((ITesterDependency)proxy);
            
            var dependencies = provider.GetDependentMembers(methodInfo).Where(x => x != null).ToList();
            var dependentMembers = dependencies
                .Where(x => x.MemberType is MemberTypes.Property or MemberTypes.Field)
                .Where(x => x.DeclaringType == testObject.GetType())
                .ToList();

            var anyIsRuntimeGenerated = dependentMembers.Any(x => x.MemberType == MemberTypes.Field
                ? ((FieldInfo)x).GetValue(testObject).GetType().FullName.StartsWith($"Lide.RuntimeGeneratedTypes.{nameof(BaseObjectProxy)}.{nameof(ITesterDependency)}")
                : ((PropertyInfo)x).GetValue(testObject).GetType().FullName.StartsWith($"Lide.RuntimeGeneratedTypes.{nameof(BaseObjectProxy)}.{nameof(ITesterDependency)}"));
            
            var dependentTypes = dependencies.Select(x => x.DeclaringType).Distinct().ToList();
            Assert.IsTrue(anyIsRuntimeGenerated);
            Assert.AreEqual(5, dependencies.Count);
            CollectionAssert.Contains(dependentTypes, typeof(ITesterDependency));
            CollectionAssert.Contains(dependentTypes, typeof(TesterWithDecoratedDependency));
            CollectionAssert.Contains(dependentTypes, typeof(DateTime));
            CollectionAssert.Contains(dependentTypes, typeof(Console));
        }

        private class TesterBroken
        {
            public void Method(Func<object> p1)
            {
                Action<object> b = Console.WriteLine;
                b(File.Exists("/home/anon/test2.txt"));
                b("");
            }
        }
        
        private class Tester
        {
            public void Method()
            {
                var e = new Exception("lqlqlq");
                Console.WriteLine(e);
                var c = Encoding.UTF8.GetBytes("ada");
            }

            private static void Internal()
            {
                Console.WriteLine("Something");
            }

        }

        private interface ITesterDependency
        {
            void Do();
        }

        private class TesterDependency : ITesterDependency
        {
            public void Do()
            {
                Console.Write("Do");
            }
        }
        
        private class TesterWithDecoratedDependency
        {
            private readonly ITesterDependency _testerDependency;
            private readonly ITesterDependency _testerDependency2;

            public TesterWithDecoratedDependency(ITesterDependency testerDependency)
            {
                _testerDependency = testerDependency;
                _testerDependency2 = testerDependency;
            }

            public void Execute()
            {
                var date = DateTime.Now;
                _testerDependency2.Do(); 
                Console.Write(date);
            }
        }
    }
}