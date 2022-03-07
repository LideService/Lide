using System;
using System.Collections.Generic;
using System.Linq;
using Lide.Core.Model;
using Lide.Core.Provider;
using Lide.Core.Provider.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Core.Tests
{
    [TestClass]
    public class TestSerializeProvider
    {
        [TestMethod]
        public void That_ValueTypeConverter_Works()
        {
            var converter = new ValueTypeConverter();
            var value1 = 19;
            var value2 = 13.7;
            var value3 = "Something";
            var value4 = 11L;
            var value5 = 7m;
            Assert.AreEqual(value1, converter.GetObject(value1.GetType(), converter.GetBytes(value1.GetType(), value1)));
            Assert.AreEqual(value2, converter.GetObject(value2.GetType(), converter.GetBytes(value2.GetType(), value2)));
            Assert.AreEqual(value3, converter.GetObject(value3.GetType(), converter.GetBytes(value3.GetType(), value3)));
            Assert.AreEqual(value4, converter.GetObject(value4.GetType(), converter.GetBytes(value4.GetType(), value4)));
            Assert.AreEqual(value5, converter.GetObject(value5.GetType(), converter.GetBytes(value5.GetType(), value5)));
        }
        
        [TestMethod]
        public void That_FieldTypeConverter_Works()
        {
            var converter = new FieldTypeConverter();
            var field1 = new byte[] { 1, 7, 13, 6, 1, 29 };
            var field3 = new byte[] { 4, 0, 255, 3 };
            var field4 = new byte[] { 14, 5, 222, 199, 18, 3 };
            var data1 = BytesMetadata.GetDefaultMetadata(13, field1);
            var data2 = BytesMetadata.GetNullMetadata(19);
            var data3 = BytesMetadata.GetMismatchMetadata(0, typeof(decimal), field3);
            var data4 = BytesMetadata.GetRootMetadata(typeof(string), field4);
            var fieldBytes1 = converter.BuildFieldBytes(data1);
            var fieldBytes2 = converter.BuildFieldBytes(data2);
            var fieldBytes3 = converter.BuildFieldBytes(data3);
            var fieldBytes4 = converter.BuildFieldBytes(data4);

            var extractedFieldBytes1 = converter.ExtractMetadata(fieldBytes1);
            var extractedFieldBytes2 = converter.ExtractMetadata(fieldBytes2);
            var extractedFieldBytes3 = converter.ExtractMetadata(fieldBytes3);
            var extractedFieldBytes4 = converter.ExtractMetadata(fieldBytes4);
            void ValidateFiledBytes(BytesMetadata expected, BytesMetadata extracted)
            {
                Assert.AreEqual(expected.MetadataType, extracted.MetadataType);
                Assert.AreEqual(expected.FieldHashCode, extracted.FieldHashCode);
                Assert.AreEqual(expected.ActualFieldType, extracted.ActualFieldType);
                if (expected.ValueBytes != null)
                    CollectionAssert.AreEqual(expected.ValueBytes, extracted.ValueBytes);
                else
                    Assert.IsNull(extracted.ValueBytes);
            }

            ValidateFiledBytes(data1, extractedFieldBytes1);
            ValidateFiledBytes(data2, extractedFieldBytes2);
            ValidateFiledBytes(data3, extractedFieldBytes3);
            ValidateFiledBytes(data4, extractedFieldBytes4);
        }
        
        [TestMethod]
        public void That_FieldsDataConverter_Works()
        {
            var converter = new BytesGlueProvider();
            var field1 = new byte[] { 1, 7, 13, 6, 1, 29 };
            var field2 = new byte[] { 14, 5, 222, 199, 18, 3 };
            var field3 = new byte[] { 4, 0, 255, 3 };
            var concat = converter.ConcatBytes(new List<byte[]>{field1, field2, field3});
            var split = converter.SplitBytes(concat);
            Assert.AreEqual(3, split.Count);
            CollectionAssert.AreEqual(field1, split.Skip(0).First());
            CollectionAssert.AreEqual(field2, split.Skip(1).First());
            CollectionAssert.AreEqual(field3, split.Skip(2).First());
        }
        
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
        public void That_SerializeAndDeserializeProduceOriginalValue()
        {
            var provider = new BinarySerializeProvider();
            var tester = new Tester()
            {
                Field1 = 7,
                Field2 = 17.8m,
                Field3 = new List<string> {"Data1", "Data2", "Data3"},
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
            Assert.AreEqual(tester.Field2, deserialized.Field2);
            Assert.AreEqual(tester.Inner.Field1, deserialized.Inner.Field1);
            Assert.IsFalse(ReferenceEquals(tester, deserialized));
            Assert.IsFalse(ReferenceEquals(tester.Inner, deserialized.Inner));
            Assert.IsFalse(ReferenceEquals(tester.Field3, deserialized.Field3));
            CollectionAssert.AreEqual(tester.Field3, deserialized.Field3);
            
            Assert.IsTrue(tester.Field4.ContainsKey("Key1"));
            Assert.IsTrue(tester.Field4.ContainsKey("Key2"));
            Assert.IsTrue(tester.Field4.ContainsKey("Key3"));
            CollectionAssert.AreEqual(tester.Field4["Key1"], new byte[] {12,15,1,240});
            CollectionAssert.AreEqual(tester.Field4["Key2"], new byte[] {});
            CollectionAssert.AreEqual(tester.Field4["Key3"], new byte[] {240, 142, 1, 0, 13, 0});
        }
        
        [TestMethod]
        public void That_SerializeAndDeserializeProduceOriginalValue_WhenAnonymousArray()
        {
            var provider = new BinarySerializeProvider();
            var tester = new Tester()
            {
                Field1 = 7,
                Field2 = 17.8m,
                Field3 = new List<string> {"Data1", "Data2", "Data3"},
                Inner = new Inner() { Field1 = "Value" }
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
            Assert.AreEqual(tester.Field2, endTester.Field2);
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
                InputParameters = new byte[] {1, 3, 2, 56, 2, 6, 7, 2, 7, 3},
            };
            var tester2 = new SubstituteMethodAfter()
            {
                CallId = 13,
                InputParameters = new byte[] {1, 3, 2, 56, 2, 6, 7, 2, 7, 3},
                IsException = true,
                OutputData = new byte[] {8, 23,5, 123, 9, 2},
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
            public int Field1 { get; init; }
            public decimal Field2 { get; init; }
            public List<string> Field3 { get; init; }
            public Dictionary<string, byte[]> Field4 { get; init; }
            public DateTime Field5 { get; init; }
            public Inner Inner { get; init; }
            public TestEnum TestEnum { get; init; }
        }

        private class Inner
        {
            public string Field1 { get; init; }
        }

        private enum TestEnum
        {
            Value1 = 7,
            Value2 = 13,
            Value3 = 45,
        }
    }
}