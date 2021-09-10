using System.Collections.Generic;
using System.Linq;
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
            Assert.AreEqual(value1, converter.GetObject(value1.GetType())(converter.GetBytes(value1.GetType())(value1)));
            Assert.AreEqual(value2, converter.GetObject(value2.GetType())(converter.GetBytes(value2.GetType())(value2)));
            Assert.AreEqual(value3, converter.GetObject(value3.GetType())(converter.GetBytes(value3.GetType())(value3)));
            Assert.AreEqual(value4, converter.GetObject(value4.GetType())(converter.GetBytes(value4.GetType())(value4)));
            Assert.AreEqual(value5, converter.GetObject(value5.GetType())(converter.GetBytes(value5.GetType())(value5)));
        }
        
        [TestMethod]
        public void That_FieldTypeConverter_Works()
        {
            var converter = new FieldTypeConverter();
            var field1 = new byte[] { 1, 7, 13, 6, 1, 29 };
            var field2 = new byte[] { 14, 5, 222, 199, 18, 3 };
            var field3 = new byte[] { 4, 0, 255, 3 };
            var fieldBytes1 = converter.BuildFieldBytes(13, field1);
            var fieldBytes2 = converter.BuildFieldBytes(19, field2);
            var fieldBytes3 = converter.BuildFieldBytes(0, field3);
            
            Assert.AreEqual(13, converter.ExtractHashCode(fieldBytes1));
            Assert.AreEqual(19, converter.ExtractHashCode(fieldBytes2));
            Assert.AreEqual(0, converter.ExtractHashCode(fieldBytes3));
            CollectionAssert.AreEqual(field1, converter.ExtractValueBytes(fieldBytes1));
            CollectionAssert.AreEqual(field2, converter.ExtractValueBytes(fieldBytes2));
            CollectionAssert.AreEqual(field3, converter.ExtractValueBytes(fieldBytes3));
        }
        
        [TestMethod]
        public void That_FieldsDataConverter_Works()
        {
            var converter = new FieldsDataConverter();
            var field1 = new byte[] { 1, 7, 13, 6, 1, 29 };
            var field2 = new byte[] { 14, 5, 222, 199, 18, 3 };
            var field3 = new byte[] { 4, 0, 255, 3 };
            var concat = converter.ConcatFieldsData(new List<byte[]>{field1, field2, field3});
            var split = converter.SplitFieldsData(concat);
            Assert.AreEqual(3, split.Count);
            CollectionAssert.AreEqual(field1, split.Skip(0).First());
            CollectionAssert.AreEqual(field2, split.Skip(1).First());
            CollectionAssert.AreEqual(field3, split.Skip(2).First());
        }
        
        [TestMethod]
        public void That_SerializeAndDeserializeProduceOriginalValue()
        {
            var provider = new SerializeProvider();
            var data = new Tester()
            {
                Field1 = 7,
                Field2 = 17.8m,
                Field3 = new List<string> {"Data1", "Data2", "Data3"},
                Inner = new Inner() { Field1 = "Value" }
            };
            
            var serialized = provider.Serialize(data);
            var deserialized = provider.Deserialize<Tester>(serialized);
            
            Assert.AreEqual(data.Field1, deserialized.Field1);
            Assert.AreEqual(data.Field2, deserialized.Field2);
            Assert.AreEqual(data.Inner.Field1, deserialized.Inner.Field1);
            Assert.IsFalse(ReferenceEquals(data, deserialized));
            Assert.IsFalse(ReferenceEquals(data.Inner, deserialized.Inner));
            Assert.IsFalse(ReferenceEquals(data.Field3, deserialized.Field3));
            CollectionAssert.AreEqual(data.Field3, deserialized.Field3);
        }

        private class Tester
        {
            public int Field1 { get; set; }
            public decimal Field2 { get; set; }

            public List<string> Field3 { get; set; }
            public Inner Inner { get; set; }
        }

        private class Inner
        {
            public string Field1 { get; set; }
        }
    }
}