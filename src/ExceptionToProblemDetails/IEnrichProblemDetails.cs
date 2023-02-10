using Microsoft.AspNetCore.Mvc;

namespace ExceptionToProblemDetails
{
    public interface IEnrichProblemDetails<in TProblemDetail> where TProblemDetail: ProblemDetails
    {
        void Enrich(TProblemDetail action);
    }
}