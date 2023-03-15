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

            var baseType = typeSymbol.BaseType;
            while (baseType != null)
            {
                if (baseType.MetadataName == typeToCheck.Name)
                    return true;

                baseType = baseType.BaseType;
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
        public static readonly DiagnosticDescriptor Ex2Pd000 = new(
            "EX2PD000",
            "Can't use top level statements with generator",
            "Main entry point can't use top level statements with ExceptionsToProblemDetails generator. Convert to class with static Main method.",
            "ExceptionToProblemDetails",
            DiagnosticSeverity.Warning,
            true
        );
    }
}