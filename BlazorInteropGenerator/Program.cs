using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorInteropGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args[0];
            var content = File.ReadAllText(fileName);
            var items = JsonConvert.DeserializeObject<WebIdlTypeDefinition[]>(content);
            foreach (var item in items)
            {
                CreateClass("CodeGenerationSample", item);
            }
        }

        /// <summary>
        /// Create a class from scratch.
        /// </summary>
        static void CreateClass(string namespaceName, WebIdlTypeDefinition token)
        {
            var type = token.Type;
            if (type != "enum")
            {
                return;
            }

            var code = GenerateEnumCode(namespaceName, token);

            // Output new code to the console.
            Console.WriteLine(code);
        }

        private static string GenerateEnumCode(string namespaceName, WebIdlTypeDefinition token)
        {
            var firstEnumMember = token.Values.FirstOrDefault();
            if (firstEnumMember.Type == "string")
            {

                return GenerateStringEnum(namespaceName, token);
            }

            return GenerateSimpleEnum(namespaceName, token);
        }

        private static string GenerateStringEnum(string namespaceName, WebIdlTypeDefinition token)
        {
            // Create a namespace: (namespace CodeGenerationSample)
            var namespaceElement = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName)).NormalizeWhitespace();

            // Add System using statement: (using System)
            namespaceElement = namespaceElement.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")));

            //  Create a class: (class Order)
            var classDeclaration = SyntaxFactory.ClassDeclaration(token.Name);

            // Add the public modifier: (public class Order)
            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
            //classDeclaration = classDeclaration.AddBaseListTypes(
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseEntity<Order>")),
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IHaveIdentity")));

            foreach (var enumMemberDefinition in token.Values)
            {
                // Create a field declaration: (private bool canceled;)
                var constField = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(NameService.GetValidIdentifier(enumMemberDefinition.Value)))
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(enumMemberDefinition.Value)))))));
                constField = constField.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                // Add the field, the property and method to the class.
                classDeclaration = classDeclaration.AddMembers(constField);
            }

            // Add the class to the namespace.
            namespaceElement = namespaceElement.AddMembers(classDeclaration);

            // Normalize and get code as string.
            var code = namespaceElement
                .NormalizeWhitespace()
                .ToFullString();
            return code;
        }

        private static string GenerateSimpleEnum(string namespaceName, WebIdlTypeDefinition token)
        {
            // Create a namespace: (namespace CodeGenerationSample)
            var namespaceElement = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName)).NormalizeWhitespace();

            // Add System using statement: (using System)
            namespaceElement = namespaceElement.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")));

            //  Create a class: (class Order)
            var classDeclaration = SyntaxFactory.EnumDeclaration(token.Name);

            // Add the public modifier: (public class Order)
            classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
            //classDeclaration = classDeclaration.AddBaseListTypes(
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseEntity<Order>")),
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IHaveIdentity")));

            foreach (var enumMemberDefinition in token.Values)
            {
                // Create a field declaration: (private bool canceled;)
                var fieldDeclaration = SyntaxFactory.EnumMemberDeclaration(enumMemberDefinition.Value);

                // Add the field, the property and method to the class.
                classDeclaration = classDeclaration.AddMembers(fieldDeclaration);
            }

            // Add the class to the namespace.
            namespaceElement = namespaceElement.AddMembers(classDeclaration);

            // Normalize and get code as string.
            var code = namespaceElement
                .NormalizeWhitespace()
                .ToFullString();
            return code;
        }
    }
}
