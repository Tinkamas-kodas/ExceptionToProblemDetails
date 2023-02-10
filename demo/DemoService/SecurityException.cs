using System;

namespace DemoService
{
    public class BaseException : Exception
    {

    }

    
    public class SecurityException : BaseException
    {
        public ForbiddenReasonEnum ForbiddenReason { get; }

        public SecurityException(ForbiddenReasonEnum forbiddenReason)
        {
            ForbiddenReason = forbiddenReason;
        }

        
    }
}