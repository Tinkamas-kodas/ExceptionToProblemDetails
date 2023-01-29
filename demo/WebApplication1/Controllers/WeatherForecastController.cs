using DemoService;
using ExceptionToProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MapToProblemDetails( StatusCodes.Status500InternalServerError,ExceptionType = typeof(Exception))]
    [ProducesResponseType(StatusCodes.Status400BadRequest )]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly WeatherForecastService forecastService;

        public WeatherForecastController(WeatherForecastService forecastService)
        {
            this.forecastService = forecastService;
        }


        [HttpGet("{location}")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast> ),StatusCodes.Status200OK )]
        [MapToProblemDetails(StatusCodes.Status404NotFound, ExceptionType = typeof(NotFoundException))]
        public IEnumerable<WeatherForecast> Get(string location)
        {
             return forecastService.GetForecastForLocation(location);
        }
        [HttpDelete("{location}")]
        [ProducesResponseType(StatusCodes.Status200OK )]
        [MapToProblemDetails(StatusCodes.Status403Forbidden,ConverterType = typeof(SecurityExceptionToSecurityProblemDetails))]
        public void Delete(string location)
        {
            if ("Vilnius".Equals(location, StringComparison.CurrentCultureIgnoreCase))
                throw new SecurityException(ForbiddenReasonEnum.NotMyEntity);
            throw new SecurityException(ForbiddenReasonEnum.EntityDeleted);
        }
    }
}