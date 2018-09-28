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

        [TestMethod]
        public void RegularTypes()
        {
            var type = JToken.Parse(@"{
                ""type"": ""return-type"",
                ""generic"": null,
                ""nullable"": null,
                ""union"": false,
                ""idlType"": ""void"",
                ""baseName"": ""void"",
                ""prefix"": null,
                ""postfix"": null,
                ""separator"": null,
                ""extAttrs"": null
            }").ToObject<WebIdlTypeReference>();
            var name = NameService.GetTypeName(type);
            Assert.AreEqual("void", name);
        }

        [TestMethod]
        public void PromiseVoid()
        {
            var type = JToken.Parse(@"{
            ""type"": ""return-type"",
            ""generic"": {
              ""value"": ""Promise"",
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
                ""idlType"": ""void"",
                ""baseName"": ""void"",
                ""prefix"": null,
                ""postfix"": null,
                ""separator"": null,
                ""extAttrs"": null,
                ""trivia"": {
                  ""base"": """"
                }
              }
            ],
            ""baseName"": ""Promise"",
            ""prefix"": null,
            ""postfix"": null,
            ""separator"": null,
            ""extAttrs"": null,
            ""trivia"": {
              ""base"": ""\r\n  ""
            }
          }").ToObject<WebIdlTypeReference>();
            var name = NameService.GetTypeName(type);
            Assert.AreEqual("Task", name);
        }

        [TestMethod]
        public void PromiseString()
        {
            var type = JToken.Parse(@"{
            ""type"": ""return-type"",
            ""generic"": {
              ""value"": ""Promise"",
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
                ""idlType"": ""string"",
                ""baseName"": ""string"",
                ""prefix"": null,
                ""postfix"": null,
                ""separator"": null,
                ""extAttrs"": null,
                ""trivia"": {
                  ""base"": """"
                }
              }
            ],
            ""baseName"": ""Promise"",
            ""prefix"": null,
            ""postfix"": null,
            ""separator"": null,
            ""extAttrs"": null,
            ""trivia"": {
              ""base"": ""\r\n  ""
            }
          }").ToObject<WebIdlTypeReference>();
            var name = NameService.GetTypeName(type);
            Assert.AreEqual("Task<string>", name);
        }
    }
}
