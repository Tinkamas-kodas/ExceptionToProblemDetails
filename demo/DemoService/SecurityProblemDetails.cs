using Microsoft.AspNetCore.Mvc;

namespace DemoService
{
    public class SecurityProblemDetails : ProblemDetails
    {
        public ForbiddenReasonEnum ForbiddenReason { get; set; }
    }

}