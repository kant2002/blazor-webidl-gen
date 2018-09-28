using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

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

        [DataTestMethod]
        [DataRow("void", "void")]
        [DataRow("string", "string")]
        [DataRow("boolean", "bool")]
        [DataRow("DOMString", "string")]
        [DataRow("USVString", "string")]
        public void RegularTypes(string idlType, string csharpType)
        {
            var type = CreateTypeDefinition(idlType);
            var name = NameService.GetTypeName(type);
            Assert.AreEqual(csharpType, name);
        }

        [TestMethod]
        public void PromiseVoid()
        {
            var type = CreateSingleParameterGeneric("Promise", "void");
            var name = NameService.GetTypeName(type);
            Assert.AreEqual("Task", name);
        }

        [TestMethod]
        public void PromiseString()
        {
            var type = CreateSingleParameterGeneric("Promise", "string");
            var name = NameService.GetTypeName(type);
            Assert.AreEqual("Task<string>", name);
        }

        [TestMethod]
        public void PromiseVoidIsAsyncCall()
        {
            var type = CreateSingleParameterGeneric("Promise", "void");
            var isAsync = NameService.IsAsync(type);
            Assert.IsTrue(isAsync);
        }

        [TestMethod]
        public void PromiseStringIsAsyncCall()
        {
            var type = CreateSingleParameterGeneric("Promise", "string");
            var isAsync = NameService.IsAsync(type);
            Assert.IsTrue(isAsync);
        }

        [TestMethod]
        public void GenericIsNotAsyncCall()
        {
            var type = CreateSingleParameterGeneric("NonPromise", "string");
            var isAsync = NameService.IsAsync(type);
            Assert.IsFalse(isAsync);
        }

        [TestMethod]
        public void StringIsNotAsyncCall()
        {
            var type = CreateTypeDefinition("string");
            var isAsync = NameService.IsAsync(type);
            Assert.IsFalse(isAsync);
        }

        [TestMethod]
        public void VoidIsNotAsyncCall()
        {
            var type = CreateTypeDefinition("void");
            var isAsync = NameService.IsAsync(type);
            Assert.IsFalse(isAsync);
        }

        private static WebIdlTypeReference CreateSingleParameterGeneric(string genericType, string arg1)
        {
            return JToken.Parse(@"{
            ""type"": ""return-type"",
            ""generic"": {
              ""value"": """ + genericType + @""",
              ""trivia"": {
                ""open"": """",
                ""close"": """"
              }
            },
            ""nullable"": null,
            ""union"": false,
            ""idlType"": [
              {
                ""type"": ""return-type"",
                ""generic"": null,
                ""nullable"": null,
                ""union"": false,
                ""idlType"": """ + arg1 + @""",
                ""baseName"": """ + arg1 + @""",
                ""prefix"": null,
                ""postfix"": null,
                ""separator"": null,
                ""extAttrs"": null,
                ""trivia"": {
                  ""base"": """"
                }
              }
            ],
            ""baseName"": """ + genericType + @""",
            ""prefix"": null,
            ""postfix"": null,
            ""separator"": null,
            ""extAttrs"": null,
            ""trivia"": {
              ""base"": ""\r\n  ""
            }
          }").ToObject<WebIdlTypeReference>();
        }

        private static WebIdlTypeReference CreateTypeDefinition(string typeName)
        {
            return JToken.Parse(@"{
                ""type"": ""return-type"",
                ""generic"": null,
                ""nullable"": null,
                ""union"": false,
                ""idlType"": """ + typeName + @""",
                ""baseName"": """ + typeName + @""",
                ""prefix"": null,
                ""postfix"": null,
                ""separator"": null,
                ""extAttrs"": null
            }").ToObject<WebIdlTypeReference>();
        }
    }
}
