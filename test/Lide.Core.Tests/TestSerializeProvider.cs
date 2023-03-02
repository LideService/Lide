using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lide.Core.Model;
using Lide.Core.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Core.Tests
{
    [TestClass]
    public class TestSerializeProvider
    {
        [TestMethod]
        public void That_LideResponse_SerializeAndDeserialize_Works()
        {
            var provider = new BinarySerializeProvider();
            var data = new LideResponse
            {
                Path = "api/something/lqlqlq",
                ContentData = new byte[] { 1, 7, 13, 6, 1, 29 },
            };

            var serialized = provider.Serialize(data);
            var deserialized = provider.Deserialize<LideResponse>(serialized);
            Assert.AreEqual(data.Path, deserialized.Path);
            CollectionAssert.AreEqual(data.ContentData, deserialized.ContentData);
        }

        [TestMethod]
        public void That_MemoryStream_Serialization_Works()
        {
            var provider = new BinarySerializeProvider();
            var value = "Something to test with";
            var data = new MemStream
            {
                MS = new MemoryStream(Encoding.UTF8.GetBytes(value))
            };

            var serialized = provider.Serialize(data);
            var deserialized = provider.Deserialize<MemStream>(serialized);
            Assert.AreEqual(value, Encoding.UTF8.GetString(deserialized.MS.ToArray()));
        }

        [TestMethod]
        public void That_SerializeAndDeserializeProduceOriginalValue()
        {

            var provider = new BinarySerializeProvider();
            var tester = new Tester(17.7m)
            {
                Field1 = 7,
                Field3 = new List<string> { "Data1", "Data2", "Data3" },
                Field4 = new Dictionary<string, byte[]>()
                {
                    {"Key1", new byte[] {12,15,1,240}},
                    {"Key2", new byte[] {}},
                    {"Key3", new byte[] {240, 142, 1, 0, 13, 0}},
                },
                Field5 = DateTime.Now,
                Inner = new Inner() { Field1 = "Value" },
                TestEnum = TestEnum.Value2,
            };

            var serialized = provider.Serialize(tester);
            var deserialized = provider.Deserialize<Tester>(serialized);

            Assert.AreEqual(tester.Field1, deserialized.Field1);
            Assert.AreEqual(tester.GetField2(), deserialized.GetField2());
            Assert.AreEqual(tester.Inner.Field1, deserialized.Inner.Field1);
            Assert.IsFalse(ReferenceEquals(tester, deserialized));
            Assert.IsFalse(ReferenceEquals(tester.Inner, deserialized.Inner));
            Assert.IsFalse(ReferenceEquals(tester.Field3, deserialized.Field3));
            CollectionAssert.AreEqual(tester.Field3, deserialized.Field3);

            Assert.IsTrue(tester.Field4.ContainsKey("Key1"));
            Assert.IsTrue(tester.Field4.ContainsKey("Key2"));
            Assert.IsTrue(tester.Field4.ContainsKey("Key3"));
            CollectionAssert.AreEqual(tester.Field4["Key1"], new byte[] { 12, 15, 1, 240 });
            CollectionAssert.AreEqual(tester.Field4["Key2"], new byte[] { });
            CollectionAssert.AreEqual(tester.Field4["Key3"], new byte[] { 240, 142, 1, 0, 13, 0 });
        }

        [TestMethod]
        public void That_SerializeAndDeserializeProduceOriginalValue_WhenAnonymousArray()
        {
            var provider = new BinarySerializeProvider();
            var tester = new Tester(17.8m)
            {
                Field1 = 7,
                Field3 = new List<string> { "Data1", "Data2", "Data3" },
                Inner = new Inner() { Field1 = "Value" },
                TestEnum = TestEnum.Value2,
            };
            var data = new object[] { 13, 7.1, 19L, "Something", tester };

            var serialized = provider.Serialize(data);
            var deserialized = provider.Deserialize<object[]>(serialized);

            Assert.AreEqual(data.Length, deserialized.Length);
            Assert.AreEqual(data[0], deserialized[0]);
            Assert.AreEqual(data[1], deserialized[1]);
            Assert.AreEqual(data[2], deserialized[2]);
            Assert.AreEqual(data[3], deserialized[3]);
            Assert.IsFalse(ReferenceEquals(data[4], deserialized[4]));
            Assert.IsTrue(deserialized[4] is Tester);

            var endTester = deserialized[4] as Tester;
            Assert.IsNotNull(endTester);
            Assert.AreEqual(tester.Field1, endTester.Field1);
            Assert.AreEqual(tester.GetField2(), endTester.GetField2());
            Assert.AreEqual(tester.TestEnum, endTester.TestEnum);
            Assert.AreEqual(tester.GetTestEnum(), endTester.GetTestEnum());
            Assert.AreEqual(tester.Inner.Field1, endTester.Inner.Field1);
            Assert.IsFalse(ReferenceEquals(tester, endTester));
            Assert.IsFalse(ReferenceEquals(tester.Inner, endTester.Inner));
            Assert.IsFalse(ReferenceEquals(tester.Field3, endTester.Field3));
            CollectionAssert.AreEqual(tester.Field3, endTester.Field3);
        }

        [TestMethod]
        public void That_SerializeAndDeserialize_OnBeforeAndAfterWorks()
        {
            var provider = new BinarySerializeProvider();
            var tester1 = new SubstituteMethodBefore()
            {
                CallId = 13,
                MethodSignature = "ValidateThis",
                InputParameters = new byte[] { 1, 3, 2, 56, 2, 6, 7, 2, 7, 3 },
            };
            var tester2 = new SubstituteMethodAfter()
            {
                CallId = 13,
                InputParameters = new byte[] { 1, 3, 2, 56, 2, 6, 7, 2, 7, 3 },
                IsException = true,
                OutputData = new byte[] { 8, 23, 5, 123, 9, 2 },
            };

            var serialized1 = provider.Serialize(tester1);
            var deserialized1 = provider.Deserialize<SubstituteMethodBefore>(serialized1);

            Assert.AreEqual(tester1.CallId, deserialized1.CallId);
            Assert.AreEqual(tester1.MethodSignature, deserialized1.MethodSignature);
            CollectionAssert.AreEqual(tester1.InputParameters, deserialized1.InputParameters);

            var serialized2 = provider.Serialize(tester2);
            var deserialized2 = provider.Deserialize<SubstituteMethodAfter>(serialized2);
            Assert.AreEqual(tester2.CallId, deserialized2.CallId);
            Assert.AreEqual(tester2.IsException, deserialized2.IsException);
            CollectionAssert.AreEqual(tester2.InputParameters, deserialized2.InputParameters);
            CollectionAssert.AreEqual(tester2.OutputData, deserialized2.OutputData);
        }

        private class Tester
        {
            public Tester(decimal f2)
            {
                Field2 = f2;
            }
            public int Field1 { get; set; }
            public TestEnum TestEnum { get; set; }
            private decimal Field2;
            public List<string> Field3 { get; set; }
            public Dictionary<string, byte[]> Field4 { get; set; }
            public DateTime Field5 { get; set; }
            public Inner Inner { get; set; }

            public TestEnum GetTestEnum() => TestEnum;
            public decimal GetField2() => Field2;
        }

        private class Inner
        {
            public string Field1 { get; set; }
        }

        private class MemStream
        {
            public MemoryStream MS { get; set; }
        }

        public enum TestEnum
        {
            Default,
            Value1 = 7,
            Value2 = 13,
            Value3 = 45,
        }
    }
}