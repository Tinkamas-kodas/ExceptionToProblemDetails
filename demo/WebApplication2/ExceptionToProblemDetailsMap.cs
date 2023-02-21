using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Opw.HttpExceptions.AspNetCore;
using Opw.HttpExceptions.AspNetCore.Mappers;

namespace WebApplication2
{

    public class ExceptionMapperWrapper<TException> : IExceptionMapper where TException : Exception
    {
        private readonly Func<HttpContext, Exception, ProblemDetailsResult>[] converters;

        public ExceptionMapperWrapper(List<Func<HttpContext, Exception, ProblemDetailsResult>> converters)
        {
            this.converters = converters.ToArray();
        }
        public bool CanMap(Type type)
        {
            return typeof(TException).IsAssignableFrom(type);
        }

        public bool TryMap(Exception exception, HttpContext context, out IStatusCodeActionResult actionResult)
        {
            actionResult = null;
            foreach (var converter in converters)
            {
                actionResult = converter.Invoke(context, exception);
                if (actionResult != null)
                    return true;
            }

            return false;
        }

        public IStatusCodeActionResult Map(Exception exception, HttpContext context)
        {
            foreach (var converter in converters)
            {
                var result = converter.Invoke(context, exception);
                if (result != null) return result;
            }

            return null;
        }
    }

    public partial class ExceptionToProblemDetailsMap
    {
        private readonly HttpExceptionsOptions options;
        private readonly List<Tuple<Type, Func<HttpContext, Exception, ProblemDetailsResult>>> converters = new List<Tuple<Type, Func<HttpContext, Exception, ProblemDetailsResult>>>();
        public ExceptionToProblemDetailsMap(HttpExceptionsOptions options)
        {
            this.options = options;
        }
        partial void MapConverter<TConverter, TException, TProblemDetails>(int statusCode, ExceptionToProblemDetails.ControllerActionDefinition actionDefinition) where TConverter : ExceptionToProblemDetails.IExceptionToProblemDetailsConverter<TException, TProblemDetails> where TException : System.Exception where TProblemDetails : ProblemDetails
        {
            var converter = Activator.CreateInstance<TConverter>();
            converters.Add(new Tuple<Type, Func<HttpContext, Exception, ProblemDetailsResult>>(typeof(TException),
                (context, exception) =>
                {
                    var ex = exception as TException;
                    if (actionDefinition.MatchRoute(context.Request.RouteValues) && ex != null)
                        return new ProblemDetailsResult(converter.Convert(ex, statusCode))
                        {
                            StatusCode = statusCode
                        };
                    return null;
                }));
            options.ExceptionMapper<TException, ExceptionMapperWrapper<TException>>(converters.Where(t => t.Item1 == typeof(TException)).Select(t => t.Item2).ToList());
        }
    }
}