
using AirTravelAggregatorAPI.Middleware;
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
               //.WriteTo.Console()
               //.WriteTo.File("logs/log.json", rollingInterval: RollingInterval.Day) 
               .ReadFrom.Configuration(builder.Configuration)
               .CreateLogger();

            Log.Information("Service start");
            builder.Host.UseSerilog();
            builder.Services.AddControllers();
            builder.Services.ConfigureApplicationServices(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
            Log.Information("Service stop");

        }
    }
}