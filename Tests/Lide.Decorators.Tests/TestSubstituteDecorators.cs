using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lide.Core.Contract.Facade;
using Lide.Core.Contract.Provider;
using Lide.Core.Facade;
using Lide.Core.Model;
using Lide.Core.Provider;
using Lide.Decorators.Substitute;
using Lide.TracingProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lide.Decorators.Tests
{
    [TestClass]
    public class TestSubstituteDecorators
    {
        [TestMethod]
        public void Do()
        {
            var fileStub = new FileFacadeStub();
            var signatureProvider = new SignatureProvider();
            var binarySerializer = new BinarySerializeProvider();
            var decorator = new SubstituteRecordDecorator(
                new CompressionProvider(),
                signatureProvider,
                binarySerializer,
                fileStub,
                new PathProvider(new PathFacade(), new ScopeIdProvider(new DateTimeFacade(), new RandomFacade()), new DateTimeFacade()),
                new TaskRunnerStub());

            var level2 = new Level2();
            var level2Proxy = ProxyDecoratorFactory.CreateProxyDecorator<ILevel2>();
            level2Proxy.SetOriginalObject(level2);
            level2Proxy.SetDecorator(decorator);
            var level2Decorated = level2Proxy.GetDecoratedObject();
            
            var level1 = new Level1(level2Decorated);
            var level1Proxy = ProxyDecoratorFactory.CreateProxyDecorator<ILevel1>();
            level1Proxy.SetOriginalObject(level1);
            level1Proxy.SetDecorator(decorator);
            var level1Decorated = level1Proxy.GetDecoratedObject();

            var result = level1Decorated.Method1("Something to test with", 24);
            var loader = new SubstituteLoader(fileStub, new CompressionProvider(), new BinarySerializeProvider());
            var signature1 = signatureProvider.GetMethodSignature(typeof(ILevel1).GetMethods().First(x => x.Name == "Method1"), SignatureOptions.AllSet);
            var signature2 = signatureProvider.GetMethodSignature(typeof(ILevel2).GetMethods().First(x => x.Name == "Method1"), SignatureOptions.AllSet);
            var signature3 = signatureProvider.GetMethodSignature(typeof(ILevel2).GetMethods().First(x => x.Name == "Method2"), SignatureOptions.AllSet);
            loader.Load("");
            var befores = loader.Befores;
            var afters = loader.Afters;

            Assert.AreEqual(22 + 15 + 24 + 13 + 22 + 15 + 24 + 17, result);
            Assert.AreEqual(3, befores.Count);
            Assert.AreEqual(1, befores[0].CallId);
            Assert.AreEqual(2, befores[1].CallId);
            Assert.AreEqual(3, befores[2].CallId);
            Assert.AreEqual(signature1, befores[0].MethodSignature);
            Assert.AreEqual(signature2, befores[1].MethodSignature);
            Assert.AreEqual(signature3, befores[2].MethodSignature);
            CollectionAssert.AreEqual(new object[] {"Something to test with", 24}, (object[])binarySerializer.Deserialize(befores[0].InputParameters));
            CollectionAssert.AreEqual(new object[] {"Something to test with.Level2.Method1", 24+13}, (object[])binarySerializer.Deserialize(befores[1].InputParameters));
            CollectionAssert.AreEqual(new object[] {"Something to test with.Level2.Method2", 24+17}, (object[])binarySerializer.Deserialize(befores[2].InputParameters));
            
            Assert.AreEqual(3, afters.Count);
            Assert.AreEqual(2, afters[0].CallId); // First Level2.Method1 finishes
            Assert.AreEqual(3, afters[1].CallId); // Next Level2.Method2 finishes
            Assert.AreEqual(1, afters[2].CallId); // Last Level1.Method1 finishes
            CollectionAssert.AreEqual(new object[] {"Something to test with", 24}, (object[])binarySerializer.Deserialize(afters[2].InputParameters));
            CollectionAssert.AreEqual(new object[] {"Something to test with.Level2.Method1", 24+13}, (object[])binarySerializer.Deserialize(afters[0].InputParameters));
            CollectionAssert.AreEqual(new object[] {"Something to test with.Level2.Method2", 24+17}, (object[])binarySerializer.Deserialize(afters[1].InputParameters));
        }

        private interface ILevel1
        {
            int Method1(string data, int value);
        }

        private interface ILevel2
        {
            int Method1(string data, int value);
            int Method2(string data, int value);
        }

        private class Level1 : ILevel1
        {
            private readonly ILevel2 _level2;

            public Level1(ILevel2 level2)
            {
                _level2 = level2;
            }

            public int Method1(string data, int value)
            {
                var a = _level2.Method1($"{data}.Level2.Method1", value + 13);
                var b = _level2.Method2($"{data}.Level2.Method2", value + 17);
                return a + b;
            }
        }

        private class Level2 : ILevel2
        {
            public int Method1(string data, int value)
            {
                return data.Length + value;
            }

            public int Method2(string data, int value)
            {
                return data.Length + value;
            }
        }

        private class TaskRunnerStub : ITaskRunner
        {
            public void AddToQueue(Task task)
            {
                task.Wait();
            }

            public Task KillQueue()
            {
                return Task.CompletedTask;
            }
        }

        private class FileFacadeStub : IFileFacade
        {
            private readonly MemoryStream _ms = new MemoryStream();
            public Task WriteToFile(string filePath, byte[] data)
            {
                _ms.Write(BitConverter.GetBytes(data.Length));
                _ms.Write(data);
                return Task.CompletedTask;
            }

            public Task<BinaryFileBatch> ReadNextBatch(string filePath, int startPosition = 0)
            {
                _ms.Seek(startPosition, SeekOrigin.Begin);
                var buffer = new byte[sizeof(int)];
                var read = _ms.Read(buffer, 0, sizeof(int));
                if (read == 0)
                {
                    return Task.FromResult(new BinaryFileBatch { Data = null, EndPosition = -1 });
                }

                var length = BitConverter.ToInt32(buffer);
                var data = new byte[length];
                _ms.Read(data, 0, length);
                return Task.FromResult(new BinaryFileBatch
                {
                    Data = data,
                    EndPosition = sizeof(int) + data.Length + startPosition,
                });
            }

            public void DeleteFile(string filePath)
            {
            }
        }
    }
}