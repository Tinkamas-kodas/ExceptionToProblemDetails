using DemoService;
using ExceptionToProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1
{
    public class SecurityExceptionEnriched : SecurityException, IEnrichProblemDetails<ProblemDetails>
    {
        public SecurityExceptionEnriched(ForbiddenReasonEnum forbiddenReason) : base(forbiddenReason)
        {
        }


        public void Enrich(ProblemDetails action)
        {
            action.Detail = ForbiddenReason.ToString();
            action.Extensions.Add("reason", ForbiddenReason);
        }
    }
    public class
        SecurityExceptionToSecurityProblemDetails : BaseExceptionToProblemDetailsConverter<SecurityException,
            SecurityProblemDetails>
    {
        public override SecurityProblemDetails Convert(SecurityException exception,int statusCode)
        {
            var details = CreateDefaultProblemDetail(statusCode);
            details.ForbiddenReason = exception.ForbiddenReason;
            return details;
        }
    }
}