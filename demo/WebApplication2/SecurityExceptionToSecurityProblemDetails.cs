using DemoService;
using ExceptionToProblemDetails;

namespace WebApplication2
{
    public class
        SecurityExceptionToSecurityProblemDetails : BaseExceptionToProblemDetailsConverter<DemoService.SecurityException,
            SecurityProblemDetails>
    {
        public override SecurityProblemDetails Convert(DemoService.SecurityException exception,int statusCode)
        {
            var details = CreateDefaultProblemDetail(statusCode);
            details.ForbiddenReason = exception.ForbiddenReason;
            return details;
        }
    }
}