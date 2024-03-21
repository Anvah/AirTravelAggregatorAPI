using AirTravelAggregatorAPI.Configurations;
using AirTravelAggregatorAPI.Configurations.Swagger;
using AirTravelAggregatorAPI.Middleware;
using Newtonsoft.Json.Converters;
using Serilog;

namespace AirTravelAggregatorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Serilog.Debugging.SelfLog.Enable(Console.Out);

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(builder.Configuration)
               .CreateLogger();

            Log.Information("Configure services start");
            builder.Host.UseSerilog();
            builder.Services.AddControllers().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    CamelCaseText = true
                });
            });
            builder.Services.ConfigureApplicationServices(builder.Configuration);
            builder.Services.ConfigureAuthentication(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.ConfigureSwagger();
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
            Log.Information("Configure services success");
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            Log.Information("Build success");
            app.Run();
            Log.Information("Service stop");

        }
    }
}