using AirTravelAggregatorAPI.Services;
using AirTravelAggregatorAPI.Services.Interfaces;
using AirTravelAggregatorAPI.Services.TestServices;
using Refit;
using Mapster;
using MapsterMapper;
using System.Linq.Expressions;
using ExpressionDebugger;
using AirTravelAggregatorAPI.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

namespace AirTravelAggregatorAPI.Configurations
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient<IFlightAggregatorService, FlightAggregateService>();
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == null)
            {
                Log.Logger.Error("failed get environment variable: ASPNETCORE_ENVIRONMENT");
            }
            if (environment != null && environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                services.AddTransient<IFirstFlightService, FirstFlightTestService>();
                services.AddTransient<ISecondFlightService, SecondFlightTestService>();
            }
            else
            {
                services.AddRefitEndpoints(configuration);
            }
            /*string? cacheSizeLimitStr = Environment.GetEnvironmentVariable("CACHE_SIZE_LIMIT");
            long cacheSizeLimit = int.MaxValue;
            if (cacheSizeLimitStr == null)
            {
                Log.Logger.Error("failed get environment variable: CACHE_SIZE_LIMIT");
            }
            else
            {
                if (long.TryParse(cacheSizeLimitStr, out cacheSizeLimit))
                    Log.Logger.Error("failed get int value from environment variable: CACHE_SIZE_LIMIT");
            }
            services.AddMemoryCache(c => c.SizeLimit = cacheSizeLimit);*/
            services.AddMemoryCache();
            services.AddSingleton(GetConfigureMappinfConfig());
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),

                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        Log.Logger.Warning("unauthorize: invalid token");
                        throw new UnauthorizedAccessException();
                    },
                };

            });
            return services;

        }
        public static IServiceCollection AddRefitEndpoints(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.GetChildren().Any(x => x.Key == nameof(RefitRoute)))
            {
                throw new ArgumentException(nameof(RefitRoute));
            }
            RefitRoute routes = ReadRoutes(configuration);

            services.AddRefitClient<IFirstFlightService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(routes.FirstFlightSevice);
                    c.Timeout = TimeSpan.FromSeconds(10);
                });
            services.AddRefitClient<ISecondFlightService>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(routes.SecondFlightSevice);
                    c.Timeout = TimeSpan.FromSeconds(10);
                });

            return services;
        }
        private static RefitRoute ReadRoutes(IConfiguration configuration)
        {
            var routes = new RefitRoute();
            var section = configuration.GetSection(nameof(RefitRoute));
            section.Bind(routes);

            return routes;
        }
        private static TypeAdapterConfig GetConfigureMappinfConfig()
        {
            var config = new TypeAdapterConfig
            {
                Compiler = exp => exp.CompileWithDebugInfo(new ExpressionCompilationOptions
                {
                    EmitFile = true,
                    ThrowOnFailedCompilation = true
                })
            };

            new MapperRegister().Register(config);

            return config;
        } 
    }
}
