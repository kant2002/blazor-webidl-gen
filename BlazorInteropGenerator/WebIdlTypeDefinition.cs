using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorInteropGenerator
{
    public class WebIdlTypeDefinition : BaseWebIdlDefinition
    {
        public string Name { get; set; }

        public WebIdlTypeReference IdlType { get; set; }

        public WebIdlArgumentDefinition[] Arguments { get; set; }
    }
}
