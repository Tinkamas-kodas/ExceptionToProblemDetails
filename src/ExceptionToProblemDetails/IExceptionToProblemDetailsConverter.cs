using System;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionToProblemDetails
{
    
    public interface IExceptionToProblemDetailsConverter<in TException, out TProblemDetails> where TException : Exception where TProblemDetails : ProblemDetails
    {
        TProblemDetails Convert(TException exception,int statusCode);
    }
}