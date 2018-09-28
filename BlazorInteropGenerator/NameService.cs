using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorInteropGenerator
{
    public static class NameService
    {
        public static string GetValidIdentifier(string webIdlIdentifier)
        {
            var parts = webIdlIdentifier.Split('-').Select(ToPascalCase);
            return string.Join("", parts);
        }

        private static string ToPascalCase(string part)
        {
            return part.Substring(0, 1).ToUpperInvariant() + part.Substring(1);
        }

        public static string GetTypeName(WebIdlTypeReference typeReference)
        {
            if (typeReference.Generic != null)
            {
                var genericType = typeReference.Generic.Value;
                switch (genericType)
                {
                    case "Promise":
                        genericType = "Task";
                        break;
                }

                var genericTypeParameter = GetTypeName(typeReference.IdlType[0]);
                var result = $"{genericType}<{genericTypeParameter}>";
                if (result == "Task<void>")
                {
                    return "Task";
                }

                return result;
            }

            return typeReference.TypeName;
        }
    }
}
