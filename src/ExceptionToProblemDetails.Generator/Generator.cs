using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace ExceptionToProblemDetails.Generator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new MapToProblemDetailsAttributeSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // the generator infrastructure will create a receiver and populate it
            // we can retrieve the populated instance via the context
            if (context.SyntaxContextReceiver is not MapToProblemDetailsAttributeSyntaxReceiver syntaxReceiver)
            {
                return;
            }
            if (syntaxReceiver.Definitions.Count==0)
                return;

            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
            if (mainMethod == null) return;
            if (mainMethod.ContainingNamespace.ToDisplayString()
                .Equals("<global namespace>", StringComparison.InvariantCultureIgnoreCase))
            {
                context.ReportDiagnostic(Diagnostic.Create(Helpers.Ex2Pd000,
                   mainMethod.Locations[0]));
                return;
            }
            var source = new StringBuilder($@"
using ExceptionToProblemDetails;
namespace {mainMethod.ContainingNamespace.ToDisplayString()}
{{
    public partial class ExceptionToProblemDetailsMap
    {{
            
            partial void MapConverter<TConverter, TException, TProblemDetails>(int statusCode, ExceptionToProblemDetails.ControllerActionDefinition actionDefinition) where TConverter : ExceptionToProblemDetails.IExceptionToProblemDetailsConverter<TException, TProblemDetails> where TException : System.Exception where TProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails;
            public void Map()
            {{
");
            foreach (var definition in syntaxReceiver.Definitions.OrderBy(t => t.InControllerName).ThenByDescending(t => t.InActionName))
            {
                if (definition.ConverterTypeFullQualifiedTypeName == null)
                {
                    source.Append($@"
                MapConverter<ExceptionToProblemDetails.BaseExceptionToProblemDetailsConverter<{definition.OnExceptionFullQualifiedTypeName},Microsoft.AspNetCore.Mvc.ProblemDetails>,{definition.OnExceptionFullQualifiedTypeName},Microsoft.AspNetCore.Mvc.ProblemDetails>({definition.StatusCode}, new ControllerActionDefinition({FormatString(definition.InControllerName)},{FormatString(definition.InActionName)}));
");
                }
                else
                {
                    source.Append($@"
                MapConverter<{definition.ConverterTypeFullQualifiedTypeName},{definition.OnExceptionFullQualifiedTypeName},{definition.ProblemDetailsTypeFullQualifiedTypeName}>({definition.StatusCode}, new ControllerActionDefinition({FormatString(definition.InControllerName)},{FormatString(definition.InActionName)}));
");
                }


            }

            source.Append(@"
            }
    }
}
");
            foreach (var syntaxReceiverError in syntaxReceiver.Errors)
            {
                context.ReportDiagnostic(syntaxReceiverError);
            }
            context.AddSource("ExceptionToProblemDetails.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
        }

        private string FormatString(string value)
        {
            return value == null ? "null" : $"\"{value}\"";
        }

        private class MapDefinition
        {
            public int StatusCode { get; set; }
            public string OnExceptionFullQualifiedTypeName { get; set; }
            public string InControllerName { get; set; }
            public string InActionName { get; set; }
            public string ConverterTypeFullQualifiedTypeName { get; set; }
            public string ProblemDetailsTypeFullQualifiedTypeName { get; set; }
        }

        private class MapToProblemDetailsAttributeSyntaxReceiver : ISyntaxContextReceiver
        {
            public List<MapDefinition> Definitions { get; } = new();
            public List<Diagnostic> Errors { get; } = new();
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax { AttributeLists: { Count: > 0 } })
                {
                    var controllerSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;
                    if (controllerSymbol == null)
                        return;
                    foreach (var attribute in controllerSymbol.GetAttributes().Where(t => t.AttributeClass?.Name == "MapToProblemDetailsAttribute"))
                    {
                        AddMapDefinition(attribute, null, controllerSymbol.Name, context.Node);
                    }
                }

                if (context.Node is MethodDeclarationSyntax { AttributeLists: { Count: > 0 } })
                {
                    if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not IMethodSymbol actionSymbol)
                        return;

                    if (context.Node.Parent == null) return;
                    if (context.SemanticModel.GetDeclaredSymbol(context.Node.Parent) is not INamedTypeSymbol controllerSymbol) return;

                    foreach (var attribute in actionSymbol.GetAttributes().Where(t => t.AttributeClass?.Name == "MapToProblemDetailsAttribute"))
                    {
                        AddMapDefinition(attribute, actionSymbol.Name, controllerSymbol.Name, context.Node);
                    }
                }
            }
            
            private void AddMapDefinition(AttributeData attribute, string actionName, string controllerName,
                SyntaxNode contextNode)
            {
                var statusCode = attribute.ConstructorArguments[0];
                if (statusCode.Value == null) return;
                if (attribute.NamedArguments.Length != 1)
                {
                    Errors.Add(Diagnostic.Create(Helpers.Ex2Pd001,
                        Location.Create(
                            contextNode.SyntaxTree,
                            TextSpan.FromBounds(contextNode.SpanStart, contextNode.SpanStart)
                        )
                    ));
                    return;
                }

                var type= attribute.NamedArguments[0].Value;
                if (type.Value is not INamedTypeSymbol typeValue)
                {
                    return;
                }
                if (attribute.NamedArguments[0].Key == "ExceptionType")
                {
                    if (typeValue .ImplementsInterfaceOrBaseClass(typeof(Exception)))
                    {
                        Definitions.Add(new MapDefinition()
                        {
                            InActionName = actionName,
                            InControllerName = controllerName,
                            StatusCode = (int)statusCode.Value,
                            OnExceptionFullQualifiedTypeName =
                                $"{typeValue.ContainingNamespace.ToDisplayString()}.{typeValue.Name}"
                        });
                    }
                    else
                    {
                        Errors.Add(Diagnostic.Create(Helpers.Ex2Pd002,
                            Location.Create(
                                contextNode.SyntaxTree,
                                TextSpan.FromBounds(contextNode.SpanStart, contextNode.SpanStart)
                            )
                        ));
                    }
                }
                else
                {
                    var converterType = typeValue.AllInterfaces.FirstOrDefault(t =>
                        t.OriginalDefinition.ToDisplayString() ==
                        "ExceptionToProblemDetails.IExceptionToProblemDetailsConverter<TException, TProblemDetails>");

                    if (converterType != null)
                    {
                        var exceptionTypeValue = converterType.TypeArguments[0];
                        var problemDetailsTypeValue = converterType.TypeArguments[1];

                        Definitions.Add(new MapDefinition()
                        {
                            InActionName = actionName,
                            InControllerName = controllerName,
                            StatusCode = (int)statusCode.Value,
                            OnExceptionFullQualifiedTypeName =
                                $"{exceptionTypeValue.ContainingNamespace.ToDisplayString()}.{exceptionTypeValue.Name}",
                            ConverterTypeFullQualifiedTypeName = typeValue.ToDisplayString(),
                            ProblemDetailsTypeFullQualifiedTypeName = problemDetailsTypeValue.ToDisplayString(),
                        });
                    }
                    else
                    {
                        Errors.Add(Diagnostic.Create(Helpers.Ex2Pd003,
                            Location.Create(
                                contextNode.SyntaxTree,
                                TextSpan.FromBounds(contextNode.SpanStart, contextNode.SpanStart)
                            )
                        ));
                    }
                }
               
            }
        }
    }
}
