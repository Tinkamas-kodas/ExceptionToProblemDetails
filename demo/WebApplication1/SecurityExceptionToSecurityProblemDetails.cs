using DemoService;
using ExceptionToProblemDetails;

namespace WebApplication1
{
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