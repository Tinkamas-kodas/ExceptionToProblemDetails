using System;

namespace DemoService
{
    public class SecurityException : Exception
    {
        public ForbiddenReasonEnum ForbiddenReason { get; }

        public SecurityException(ForbiddenReasonEnum forbiddenReason)
        {
            ForbiddenReason = forbiddenReason;
        }

        
    }
}