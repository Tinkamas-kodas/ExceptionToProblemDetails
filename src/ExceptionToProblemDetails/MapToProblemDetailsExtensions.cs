using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionToProblemDetails
{
    

    public static class MapToProblemDetailsExtensions
    {
        public class ExceptionMapBuilder<TBuilder>
        {
            private readonly List<MapToProblemDetailsAttribute> attributes = new();
            public ExceptionMapBuilder<TBuilder> MapException<TException>(int statusCode) 
                where TException : Exception
            {
                attributes.Add(new MapToProblemDetailsAttribute(statusCode)
                {
                    ConverterType = typeof(BaseExceptionToProblemDetailsConverter<TException, ProblemDetails>)
                });
                return this;
            }
            public ExceptionMapBuilder<TBuilder> MapException<TException, TProblemDetails>(int statusCode) 
                where TException : Exception 
                where TProblemDetails : ProblemDetails, new()
            {
                attributes.Add(new MapToProblemDetailsAttribute(statusCode)
                {
                    ConverterType = typeof(BaseExceptionToProblemDetailsConverter<TException, TProblemDetails>)
                });
                return this;
            }
            public ExceptionMapBuilder<TBuilder> MapConverter<TConverter, TException>(int statusCode) 
                where TConverter: IExceptionToProblemDetailsConverter<TException, ProblemDetails>
                where TException: Exception
            {
                attributes.Add(new MapToProblemDetailsAttribute(statusCode)
                {
                    ConverterType = typeof(TConverter)
                });
                return this;
            }

            internal void AddToEndpointMetaData(IList<object> endpointBuilderMetadata)
            {
                foreach (var mapToProblemDetailsAttribute in attributes)
                {
                    endpointBuilderMetadata.Add(mapToProblemDetailsAttribute);
                }
            }
        }
        public static TBuilder ProduceProblemDetails<TBuilder>(this TBuilder builder, Action<ExceptionMapBuilder<TBuilder>> options)
            where TBuilder : IEndpointConventionBuilder
        {
            var exceptionMapBuilder = new ExceptionMapBuilder<TBuilder>();
            options.Invoke(exceptionMapBuilder);
            builder.Add(endpointBuilder =>
            {
                exceptionMapBuilder.AddToEndpointMetaData(endpointBuilder.Metadata);
                
            });
            return builder;
        }
        
        
    }
}