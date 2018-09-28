using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorInteropGenerator
{
    public class WebIdlBodyDefinition : BaseWebIdlDefinition
    {
        public DetailedNameDescription Name { get; set; }

        public WebIdlTypeReference IdlType { get; set; }

        public WebIdlArgumentDefinition[] Arguments { get; set; }
    }
}
