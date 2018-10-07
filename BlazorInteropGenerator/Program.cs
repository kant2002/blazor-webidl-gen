using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlazorInteropGenerator
{
    class Program
    {
        /// <summary>
        /// Gets or sets source file to process.
        /// </summary>
        [Option("--source-file|-s")]
        [Required]
        [FileExists]
        public string SourceFile { get; set; }

        /// <summary>
        /// Gets or sets name of the namespace where extensions would be generated.
        /// </summary>
        [Option("--namespace|-n")]
        public string NamespaceName { get; set; } = "CodeGenerationSample";

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments passed to the application.</param>
        /// <returns>Exit code for the application.</returns>
        static int Main(string[] args)
        {
            var fileName = args[0];
            return CommandLineApplication.Execute<Program>(args);
        }

        private int OnExecute(IConsole console)
        {
            var content = File.ReadAllText(this.SourceFile);
            var items = JsonConvert.DeserializeObject<WebIdlTypeDefinition[]>(content);
            CreateClassDefinitions(this.NamespaceName, items);
            return 0;
        }

        private static void CreateClassDefinitions(string namespaceName, WebIdlTypeDefinition[] items)
        {
            var namespaceElement = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName)).NormalizeWhitespace()
                .WithUsings(
                    SyntaxFactory.List(new UsingDirectiveSyntax[]
                    {
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                    }));

            foreach (var item in items)
            {
                var memberDeclaration = GetTypeDefinitionCode(item);
                if (memberDeclaration != null)
                {
                    // Add the class to the namespace.
                    namespaceElement = namespaceElement.AddMembers(memberDeclaration);
                }
            }

            // Normalize and get code as string.
            var code = namespaceElement
                .NormalizeWhitespace()
                .ToFullString();
            Console.WriteLine(code);
        }

        private static MemberDeclarationSyntax GetTypeDefinitionCode(WebIdlTypeDefinition token)
        {
            MemberDeclarationSyntax memberDeclaration = null;
            var type = token.Type;
            if (type == "enum")
            {
                memberDeclaration = GenerateEnumCode(token);
            }

            if (type == "interface")
            {
                memberDeclaration = GenerateInterfaceCode(token);
            }

            return memberDeclaration;
        }

        private static ClassDeclarationSyntax GenerateInterfaceCode(WebIdlTypeDefinition token)
        {
            var name = token.Name;
            try
            {
                var classDeclaration = SyntaxFactory.ClassDeclaration(name)
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(
                                SyntaxKind.PublicKeyword)));

                //// Inherit BaseEntity<T> and implement IHaveIdentity: (public class Order : BaseEntity<T>, IHaveIdentity)
                //classDeclaration = classDeclaration.AddBaseListTypes(
                //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseEntity<Order>")),
                //    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IHaveIdentity")));

                var proxyClass = $"Blazor{token.Name}Proxy";
                foreach (var enumMemberDefinition in token.Members)
                {
                    var constField = CreateInterfaceMember(proxyClass, enumMemberDefinition);

                    // Add the field, the property and method to the class.
                    classDeclaration = classDeclaration.AddMembers(constField);
                }

                return classDeclaration;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during generation interface {name}", ex);
            }
        }

        private static MemberDeclarationSyntax CreateInterfaceMember(string proxyClass, WebIdlMemberDefinition memberDefinition)
        {
            // var name = memberDefinition.Body;
            if (memberDefinition.Type == "operation")
            {
                return CreateInterfaceOperation(proxyClass, memberDefinition);
            }

            if (memberDefinition.Type == "attribute")
            {
                return CreateInterfaceAttribute(memberDefinition);
            }

            throw new InvalidDataException($"The member type {memberDefinition.Type} is not supported.");
        }

        private static MemberDeclarationSyntax CreateInterfaceOperation(string proxyClass, WebIdlMemberDefinition memberDefinition)
        {
            var name = memberDefinition.Body.Name.Escaped;
            try
            {
                var returnTypeReference = memberDefinition.Body.IdlType;
                var isAsyncCall = NameService.IsAsync(returnTypeReference);
                if (isAsyncCall)
                {
                    name += "Async";
                }

                var returnType = CreateType(returnTypeReference);
                var arguments = memberDefinition.Body.Arguments.Select(CreateParameter);
                var body = isAsyncCall
                    ? GenerateAsyncProxyCall(proxyClass, memberDefinition)
                    : GenerateSyncProxyCall(proxyClass, memberDefinition);
                return SyntaxFactory.MethodDeclaration(returnType, NameService.GetValidIdentifier(name))
                    .WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                    .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(arguments)))
                    .WithBody(body);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during generation operation {name}", ex);
            }
        }

        private static MemberDeclarationSyntax CreateInterfaceAttribute(WebIdlMemberDefinition memberDefinition)
        {
            var name = memberDefinition.Name;
            try
            {
                var constField = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName(NameService.GetTypeName(memberDefinition.IdlType)))
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(NameService.GetValidIdentifier(memberDefinition.Name))))))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
                return constField;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during generation atribute {name}", ex);
            }
        }

        private static BlockSyntax GenerateAsyncProxyCall(string proxyClass, WebIdlMemberDefinition memberDefinition)
        {
            /*
            public static async Task<string> MethodNameAsync(string arg1, string arg2)
            {
                var asyncJsRunTime = JSRuntime.Current;
                return await asyncJsRunTime.InvokeAsync<string>("proxyClass.methodName", arg1, arg2);
            }
            */
            var methodName = memberDefinition.Body.Name.Escaped;
            var asyncInvocation = SyntaxFactory.ParseExpression($@"await asyncJsRunTime.InvokeAsync<string>(""{proxyClass}.{methodName}"", message)");
            var syntax = SyntaxFactory.ParseStatement("var asyncJsRunTime = JSRuntime.Current;");
            var proxyFunctionArgument = SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal($"{proxyClass}.{methodName}")));
            var arguments = memberDefinition.Body.Arguments.Select(CreateArgumentCallExpression);
            var proxyCallArguments = new[] { proxyFunctionArgument }.Union(arguments);

            var typeName = memberDefinition.Body.IdlType.TypeName ?? memberDefinition.Body.IdlType.IdlType[0].TypeName;
            var resultType = typeName == "void" 
                ? SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                : SyntaxFactory.ParseTypeName(NameService.GetTypeName(memberDefinition.Body.IdlType));
            var awaitExpress = SyntaxFactory.AwaitExpression(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        kind: SyntaxKind.SimpleMemberAccessExpression,
                        expression: SyntaxFactory.IdentifierName("asyncJsRunTime"),
                        name: SyntaxFactory.GenericName(
                            identifier: SyntaxFactory.Identifier("InvokeAsync"),
                            typeArgumentList: SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(resultType)))),
                    argumentList: SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            proxyCallArguments))));
            return SyntaxFactory.Block(
                syntax,
                SyntaxFactory.ReturnStatement(awaitExpress));
        }

        private static BlockSyntax GenerateSyncProxyCall(string proxyClass, WebIdlMemberDefinition memberDefinition)
        {
            /*
            public static async Task<string> MethodNameAsync(string arg1, string arg2)
            {
                var asyncJsRunTime = JSRuntime.Current;
                return await asyncJsRunTime.InvokeAsync<string>("proxyClass.methodName", arg1, arg2);
            }
            */
            var methodName = memberDefinition.Body.Name.Escaped;
            var asyncInvocation = SyntaxFactory.ParseExpression($@"await asyncJsRunTime.InvokeAsync<string>(""{proxyClass}.{methodName}"", message)");
            var syntax = SyntaxFactory.ParseStatement("var syncJsRunTime = (IJSInProcessRuntime)JSRuntime.Current;");
            var proxyFunctionArgument = SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal($"{proxyClass}.{methodName}")));
            var arguments = memberDefinition.Body.Arguments.Select(CreateArgumentCallExpression);
            var proxyCallArguments = new[] { proxyFunctionArgument }.Union(arguments);

            var typeName = memberDefinition.Body.IdlType.TypeName ?? memberDefinition.Body.IdlType.IdlType[0].TypeName;
            var resultType = typeName == "void"
                ? SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                : SyntaxFactory.ParseTypeName(NameService.GetTypeName(memberDefinition.Body.IdlType));
            var awaitExpress = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        kind: SyntaxKind.SimpleMemberAccessExpression,
                        expression: SyntaxFactory.IdentifierName("syncJsRunTime"),
                        name: SyntaxFactory.GenericName(
                            identifier: SyntaxFactory.Identifier("Invoke"),
                            typeArgumentList: SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(resultType)))),
                    argumentList: SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            proxyCallArguments)));
            return SyntaxFactory.Block(
                syntax,
                SyntaxFactory.ReturnStatement(awaitExpress));
        }

        private static ArgumentSyntax CreateArgumentCallExpression(WebIdlArgumentDefinition argumentDefinition)
        {
            return SyntaxFactory.Argument(
                SyntaxFactory.IdentifierName(argumentDefinition.Name));
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
            var namespaceElement = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName)).NormalizeWhitespace()
                .WithUsings(
                    SyntaxFactory.List(new UsingDirectiveSyntax[]
                    {
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                    }));

            var memberDeclaration = GenerateEnumCode(token);

            namespaceElement = namespaceElement.AddMembers(memberDeclaration);
            // Normalize and get code as string.
            var code = namespaceElement
                .NormalizeWhitespace()
                .ToFullString();
            return code;
        }

        private static MemberDeclarationSyntax GenerateEnumCode(WebIdlTypeDefinition token)
        {
            var firstEnumMember = token.Values.FirstOrDefault();
            MemberDeclarationSyntax memberDeclaration;
            if (firstEnumMember.Type == "string")
            {
                memberDeclaration = GenerateStringEnum(token);
            }
            else
            {
                memberDeclaration = GenerateSimpleEnum(token);
            }

            return memberDeclaration;
        }

        private static ClassDeclarationSyntax GenerateStringEnum(WebIdlTypeDefinition token)
        {
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

            return classDeclaration;
        }

        private static EnumDeclarationSyntax GenerateSimpleEnum(WebIdlTypeDefinition token)
        {
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

            return classDeclaration;
        }
    }
}
