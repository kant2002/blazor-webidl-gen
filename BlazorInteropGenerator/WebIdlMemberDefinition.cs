using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorInteropGenerator
{
    public class WebIdlMemberDefinition : BaseWebIdlDefinition
    {
        public string Name { get; set; }

        public WebIdlBodyDefinition Body { get; set; }

        public WebIdlTypeReference IdlType { get; set; }
    }
}
