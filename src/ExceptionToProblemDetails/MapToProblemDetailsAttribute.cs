using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace ExceptionToProblemDetails
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MapToProblemDetailsAttribute:Attribute, IApiResponseMetadataProvider
    {
        private Type converterType;
        public Type Type { get; private set; } = typeof(ProblemDetails);

        public int StatusCode { get; }
        public Type ExceptionType { get; set; }

        public Type ConverterType
        {
            get => converterType;
            
            set 
            {
                var interfaceType = value.GetInterfaces().FirstOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IExceptionToProblemDetailsConverter<,>));
                if (interfaceType == null)
                    throw new ArgumentException("Value must implement IExceptionToProblemDetailsConverter<,>");
                converterType = value;
                ExceptionType = interfaceType.GetGenericArguments()[0];
                Type = interfaceType.GetGenericArguments()[1];
            }
        }

        public MapToProblemDetailsAttribute(int statusCode)
        {
            StatusCode = statusCode;
        }

        void IApiResponseMetadataProvider.SetContentTypes(MediaTypeCollection contentTypes)
        {
            //nop
        }
        
    }
}
