using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorInteropGenerator
{
    class WebIdlTypeDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public WebIdlTypeDefinition[] Values { get; set; }

        [JsonExtensionData]
        internal IDictionary<string, JToken> ExtraStuff { get; set; }
    }
}
