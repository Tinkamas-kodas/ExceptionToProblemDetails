using Hellang.Middleware.ProblemDetails;

namespace WebApplication1
{
    public partial class ExceptionToProblemDetailsMap
    {
        private readonly ProblemDetailsOptions options;

        public ExceptionToProblemDetailsMap(ProblemDetailsOptions options)
        {
            this.options = options;
        }

        partial void MapConverter<TConverter, TException, TProblemDetails>(int statusCode, ExceptionToProblemDetails.ControllerActionDefinition actionDefinition) where TConverter : ExceptionToProblemDetails.IExceptionToProblemDetailsConverter<TException, TProblemDetails> where TException : System.Exception where TProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            var converter = Activator.CreateInstance<TConverter>();
            options.Map<TException>((context, exception) =>
                actionDefinition.MatchRoute(context.Request.RouteValues)
                    ? converter.Convert(exception, statusCode)
                    : null);
        }
    }
}