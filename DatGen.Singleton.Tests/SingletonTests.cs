using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatGen.Singleton.Tests
{
    [TestClass]
    public class SingletonTests
    {
        [TestMethod]
        public void Singleton_GetInstance_ReturnsInstance()
        {
            var fakeSingleton = FakeSingleton.Instance;

            Assert.IsNotNull(fakeSingleton);
            Assert.IsInstanceOfType(fakeSingleton, typeof(FakeSingleton));
        }

        [TestMethod]
        public void Singleton_GetInstanceRandomTimes_CheckInstanceCount()
        {
            int instanceCount = (new Random()).Next(1, 999);
            while(instanceCount-- > 0)
            { 
                var fakeSingleton = FakeSingleton.Instance;
            }

            var expectedInstanceCount = 1;
            var actualInstanceCount = FakeSingleton.InstanceCount;

            Assert.AreEqual(expectedInstanceCount, actualInstanceCount);
        }
    }
}
