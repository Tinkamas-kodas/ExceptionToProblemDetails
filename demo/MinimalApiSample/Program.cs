using DemoService;

using ExceptionToProblemDetails;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

using SecurityException = MinimalApiSample.SecurityException;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.EnableTryItOutByDefault();
        o.DisplayRequestDuration();
    });
}

app.Use(async (context, @delegate) =>
{
    try
    {
        await @delegate.Invoke();
    }
    catch (Exception ex)
    {
        var endpointFeature = (IEndpointFeature)context.Features[typeof(IEndpointFeature)]!;
        var mapAttribute = endpointFeature.Endpoint?.Metadata
                .Where(t => t is MapToProblemDetailsAttribute)
                .Cast<MapToProblemDetailsAttribute>()
                .FirstOrDefault(t => ex.GetType().IsAssignableTo(t.ExceptionType));

        if (mapAttribute != null)
        {
            var handler = Activator.CreateInstance(mapAttribute.ConverterType);

            var problemDetails = mapAttribute.ConverterType.GetMethod("Convert")?.Invoke(handler, new object[] { ex, mapAttribute.StatusCode });

            context.Response.StatusCode = mapAttribute.StatusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);

        }
        else
        {
            var handler = new BaseExceptionToProblemDetailsConverter<Exception, ProblemDetails>();
            var problemDetails = handler.Convert(ex, 500);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }


    }

});

app.MapGet("/weatherforecast/{location}", (string? location, [FromServices] WeatherForecastService forecastService)
        => forecastService.GetForecastForLocation(location))
.WithName("GetWeatherForecast")
.ProduceProblemDetails(mapBuilder => mapBuilder.MapException<NotFoundException>(404))
.WithOpenApi();


app.MapDelete("/weatherforecast/{location}", (string location, [FromServices] WeatherForecastService forecastService)
        =>
    {
        if ("Vilnius".Equals(location, StringComparison.CurrentCultureIgnoreCase))
            throw new SecurityException(ForbiddenReasonEnum.NotMyEntity);
        throw new NotFoundException();
    })
    .WithName("DeleteWeatherForecast")
    .ProduceProblemDetails(mapBuilder =>
        mapBuilder.MapException<SecurityException, SecurityProblemDetails>(403)
        .MapConverter<BaseExceptionToProblemDetailsConverter<NotFoundException, ProblemDetails>, NotFoundException>(404))
    .WithOpenApi();


app.Run();