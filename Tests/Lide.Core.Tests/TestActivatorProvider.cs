using System;
using Lide.Core.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Core.Tests
{
    [TestClass]
    public class TestActivatorProvider
    {
        [TestMethod]
        public void That_CreateObjectWorks_When_PrivateConstructorWithParameters()
        {
            var provider = new ActivatorProvider();
            var tester = (Tester)provider.CreateObject(typeof(Tester));
            var array = (int[])provider.CreateObject(typeof(int[]));
            Assert.IsNotNull(tester);
            Assert.IsNull(tester.Inner);
            Assert.IsNull(tester.Data1);
            Assert.IsNull(tester.Data3);
            Assert.IsNotNull(array);
            Assert.AreEqual(0, array.Length);
            Assert.AreEqual(0, tester.Data2);
        }
        
        [TestMethod]
        public void That_DeepCopyIntoExistingObjectCopiesAllFields()
        {
            var provider = new ActivatorProvider();
            var array = new[] { 1, 6, 7, 8, 2 };
            var source = Tester.GetInstance(Inner.GetInstance("Test1"), "Test2", 13, array);
            var target = Tester.GetInstance(null, null, 0, Array.Empty<int>());

            provider.DeepCopyIntoExistingObject(source, target);
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Inner);
            Assert.AreEqual("Test2", target.Data1);
            Assert.AreEqual(13, target.Data2);
            Assert.AreEqual(5, target.Data3.Length);
            CollectionAssert.AreEqual(array, target.Data3);
            Assert.AreEqual("Test1", target.Inner.Data);
            Assert.IsFalse(ReferenceEquals(source.Inner, target.Inner));
        }
        
        [TestMethod]
        public void That_ReferenceObjectsAreKeptWithTheSameReference()
        {
            var provider = new ActivatorProvider();
            var targetInner = Inner.GetInstance("Test3");
            var source = Tester.GetInstance(Inner.GetInstance("Test1"), "Test2", 13, new[] { 1, 6, 7, 8, 2 });
            var target = Tester.GetInstance(targetInner, null, 0, Array.Empty<int>());

            provider.DeepCopyIntoExistingObject(source, target);
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Inner);
            Assert.AreEqual("Test2", target.Data1);
            Assert.AreEqual(13, target.Data2);
            Assert.AreEqual("Test1", target.Inner.Data);
            Assert.IsFalse(ReferenceEquals(source.Inner, target.Inner));
            Assert.IsTrue(ReferenceEquals(targetInner, target.Inner));
        }

        private class Tester
        {
            public Inner Inner { get; }
            public string Data1 { get; }
            public int Data2 { get; }
            public int[] Data3 { get; }

            public static Tester GetInstance(Inner inner, string data1, int data2, int[] data3) => new Tester(inner, data1, data2, data3);
            private Tester(Inner inner, string data1, int data2, int[] data3)
            {
                Inner = inner;
                Data1 = data1;
                Data2 = data2;
                Data3 = data3;
            }
        }

        private class Inner
        {
            public string Data { get; }

            public static Inner GetInstance(string data) => new Inner(data);
            private Inner(string data)
            {
                Data = data;
            }
        }
    }
}