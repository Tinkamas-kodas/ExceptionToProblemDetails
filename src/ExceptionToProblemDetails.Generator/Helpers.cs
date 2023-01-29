using System;
using Microsoft.CodeAnalysis;

namespace ExceptionToProblemDetails.Generator
{
    public static class Helpers
    {
        public static bool ImplementsInterfaceOrBaseClass(this INamedTypeSymbol typeSymbol, Type typeToCheck)
        {
            if (typeSymbol == null)
            {
                return false;
            }

            if (typeSymbol.MetadataName == typeToCheck.Name)
            {
                return true;
            }

            if (typeSymbol.BaseType != null && typeSymbol.BaseType.MetadataName == typeToCheck.Name)
            {
                return true;
            }

            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                if (@interface.MetadataName == typeToCheck.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public static readonly DiagnosticDescriptor Ex2Pd001 = new(
            "EX2PD001",
            "Invalid MapToProblemDetailsAttribute usage.",
            "MapToProblemDetailsAttribute must contain only ExceptionType or only ConverterType named argument.",
            "ExceptionToProblemDetails",
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor Ex2Pd002 = new(
            "EX2PD002",
            "Invalid ExceptionType argument",
            "ExceptionType argument must contain type derived from System.Exception.",
            "ExceptionToProblemDetails",
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor Ex2Pd003 = new(
            "EX2PD003",
            "Invalid ConverterType argument",
            "ConverterType argument must implement interface IExceptionToProblemDetailsConverter<TException, TProblemDetails>.",
            "ExceptionToProblemDetails",
            DiagnosticSeverity.Error,
            true
        );
    }
}