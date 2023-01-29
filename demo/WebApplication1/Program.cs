using System.Text.Json.Serialization;
using DemoService;
using Hellang.Middleware.ProblemDetails;
using Serilog;
using Serilog.Events;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console()
                .CreateLogger();
            try
            {
                Log.Information("Starting web application");
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();
                // Add services to the container.

                builder.Services
                    .AddControllers().AddJsonOptions(opts =>
                    {
                        var enumConverter = new JsonStringEnumConverter();
                        opts.JsonSerializerOptions.Converters.Add(enumConverter);
                    })
                    //.AddProblemDetailsConventions()
                    ;
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddTransient<WeatherForecastService>();
                builder.Services.AddProblemDetails(options =>
                {
                    new ExceptionToProblemDetailsMap(options).Map();
                    options.ShouldLogUnhandledException = (context, exception, arg3) => false;
                    options.IncludeExceptionDetails = (context, exception) => false;
                });

                var app = builder.Build();
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                
                app.UseSerilogRequestLogging();
                app.UseProblemDetails();
                 
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}