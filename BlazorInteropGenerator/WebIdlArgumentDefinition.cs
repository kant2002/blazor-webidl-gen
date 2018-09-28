using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorInteropGenerator
{
    public class WebIdlArgumentDefinition : BaseWebIdlDefinition
    {
        public string Name { get; set; }
        public string EscapedName { get; set; }

        public WebIdlTypeReference IdlType { get; set; }
    }
}
