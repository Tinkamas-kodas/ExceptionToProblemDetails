using DemoService;
using ExceptionToProblemDetails;

namespace MinimalApiSample;

public class SecurityException:DemoService.SecurityException, IEnrichProblemDetails<SecurityProblemDetails>
{
    public SecurityException(ForbiddenReasonEnum forbiddenReason) : base(forbiddenReason)
    {
    }

    public void Enrich(SecurityProblemDetails action)
    {
        action.ForbiddenReason = this.ForbiddenReason;
    }
}

