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
            var code = GetTypeDefinitionCode(namespaceName, token);

            // Output new code to the console.
            Console.WriteLine(code);
        }

        /// <summary>
        /// Create a class from scratch.
        /// </summary>
        /// <param name="namespaceName">Name of namespace</param>
        /// <param name="token">Web IDL token.</param>
        static string GetTypeDefinitionCode(string namespaceName, WebIdlTypeDefinition token)
        {
            var type = token.Type;
            if (type == "enum")
            {
                return GenerateEnumCode(namespaceName, token);
            }

            if (type == "interface")
            {
                return GenerateInterfaceCode(namespaceName, token);
            }

            return string.Empty;
        }

        private static string GenerateInterfaceCode(string namespaceName, WebIdlTypeDefinition token)
        {
            var namespaceElement = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName)).NormalizeWhitespace()
                .WithUsings(
                    SyntaxFactory.List(new UsingDirectiveSyntax[]
                    {
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                    }));

            var classDeclaration = SyntaxFactory.ClassDeclaration(token.Name)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(
                    SyntaxKind.PublicKeyword)));

            //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
            //classDeclaration = classDeclaration.AddBaseListTypes(
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseEntity<Order>")),
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IHaveIdentity")));

            foreach (var enumMemberDefinition in token.Members)
            {
                var constField = CreateInterfaceMember(enumMemberDefinition);

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

        private static MemberDeclarationSyntax CreateInterfaceMember(WebIdlMemberDefinition memberDefinition)
        {
            // var name = memberDefinition.Body;
            if (memberDefinition.Type == "operation")
            {
                var name = memberDefinition.Body.Name.Escaped;
                var returnTypeReference = memberDefinition.Body.IdlType;
                var returnType = CreateType(returnTypeReference);
                var arguments = memberDefinition.Body.Arguments.Select(CreateParameter);
                return SyntaxFactory.MethodDeclaration(returnType, name)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)));
            }

            if (memberDefinition.Type == "attribute")
            {
                var name = memberDefinition.Name;
                var constField = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(NameService.GetValidIdentifier(memberDefinition.Name))))))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
                return constField;
            }

            throw new InvalidDataException($"The member type {memberDefinition.Type} is not supported.");
        }

        private static TypeSyntax CreateType(WebIdlTypeReference returnTypeReference)
        {
            var typeName = NameService.GetTypeName(returnTypeReference);
            var returnType = SyntaxFactory.ParseTypeName(typeName);
            return returnType;
        }

        private static ParameterSyntax CreateParameter(WebIdlArgumentDefinition arg)
        {
            return SyntaxFactory.Parameter(SyntaxFactory.Identifier(arg.EscapedName))
                .WithType(CreateType(arg.IdlType));
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
            var classDeclaration = SyntaxFactory.ClassDeclaration(token.Name)
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

            var enumType = SyntaxFactory.ParseTypeName(token.Name);
            //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
            //classDeclaration = classDeclaration.AddBaseListTypes(
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseEntity<Order>")),
            //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IHaveIdentity")));

            var backingField = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier("field")))))
                    .WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));
            classDeclaration = classDeclaration.AddMembers(backingField);

            var constructorMember = SyntaxFactory.ConstructorDeclaration(token.Name)
                    .WithParameterList(SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                            SyntaxFactory.Parameter(
                                SyntaxFactory.Identifier("value"))
                                .WithType(SyntaxFactory.PredefinedType(
                                    SyntaxFactory.Token(SyntaxKind.StringKeyword))))))
                    .WithExpressionBody(
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName("field"),
                                SyntaxFactory.IdentifierName("value"))))
                    .WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            classDeclaration = classDeclaration.AddMembers(constructorMember);

            foreach (var enumMemberDefinition in token.Values)
            {
                var backingValue = SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(enumMemberDefinition.Value));
                var initializationValue = SyntaxFactory.ObjectCreationExpression(enumType)
                    .WithArgumentList(SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                            SyntaxFactory.Argument(backingValue))));
                var constField = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        enumType)
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(NameService.GetValidIdentifier(enumMemberDefinition.Value)))
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(
                                    initializationValue)))))
                    .WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword),
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)));

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
