using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ExceptionToProblemDetails
{
    public class BaseExceptionToProblemDetailsConverter<TException, TProblemDetail> : IExceptionToProblemDetailsConverter<TException, TProblemDetail> where TException : Exception where TProblemDetail: ProblemDetails, new()
    {
        public virtual TProblemDetail Convert(TException exception, int statusCode)
        {
            return CreateDefaultProblemDetail(statusCode);
        }

        protected TProblemDetail CreateDefaultProblemDetail(int statusCode)
        {
            return new TProblemDetail
            {
                Status = statusCode,
                Type = $"https://httpstatuses.io/{statusCode}",
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
            };
        }
    }


}