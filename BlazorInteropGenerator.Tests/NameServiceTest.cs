using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlazorInteropGenerator.Tests
{
    [TestClass]
    public class NameServiceTest
    {
        [TestMethod]
        public void NamesConvertedToPascalCase()
        {
            var name = NameService.GetValidIdentifier("default");
            Assert.AreEqual("Default", name);
        }

        [TestMethod]
        public void DashNamesConvertedToPascalCase()
        {
            var name = NameService.GetValidIdentifier("no-audio");
            Assert.AreEqual("NoAudio", name);
        }
    }
}
