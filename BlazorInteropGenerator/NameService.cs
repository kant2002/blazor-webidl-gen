﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebIdlCSharp;

namespace BlazorInteropGenerator
{
    public static class NameService
    {
        public static string ConvertFromWebIdlIdentifier(string webIdlIdentifier)
        {
            var parts = webIdlIdentifier.Split('-').Select(ToPascalCase);
            return string.Join("", parts);
        }

        public static string GetValidIndentifier(string identifier)
        {
            var reservedWords = new HashSet<string>
                {
                    { "string" },
                    { "default" },
                };
            if (reservedWords.Contains(identifier))
            {
                return "@" + identifier;
            }

            return identifier;
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
                var wellKnownGenericTypeOverrides = new Dictionary<string, string>
                {
                    { "Promise", "Task" },
                    { "sequence", "IEnumerable" },
                    { "FrozenArray", "IReadOnlyList" },
                };
                if (wellKnownGenericTypeOverrides.TryGetValue(genericType, out var overrideTypeName))
                {
                    genericType = overrideTypeName;
                }

                var genericTypeParameter = GetTypeName(typeReference.IdlType[0]);
                var result = $"{genericType}<{genericTypeParameter}>";
                if (result == "Task<void>")
                {
                    return "Task";
                }

                return result;
            }

            var wellKnownTypeOverrides = new Dictionary<string, string>
            {
                { "boolean", "bool" },
                { "DOMString", "string" },
                { "USVString", "string" },
                { "unsigned short", "ushort" },
                { "unsigned long", "ulong" },
                { "unsigned long long", "ulong" },
                { "any", "object" },
                { "DOMTimeStamp", "DateTime" },
                { "DOMHighResTimeStamp", "double" },

                // Temporary workaround until generator would be fully implemented.
                { "VibratePattern", "int[]" },
            };
            if (wellKnownTypeOverrides.TryGetValue(typeReference.TypeName, out var overrideType))
            {
                return overrideType;
            }

            return typeReference.TypeName;
        }

        public static bool IsAsync(WebIdlTypeReference type)
        {
            if (type.Generic?.Value == "Promise")
            {
                return true;
            }

            return false;
        }
    }
}
