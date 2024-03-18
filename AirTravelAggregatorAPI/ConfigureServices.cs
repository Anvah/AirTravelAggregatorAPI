using AirTravelAggregatorAPI.Services;
using AirTravelAggregatorAPI.Services.Interfaces;
using AirTravelAggregatorAPI.Services.TestServices;
using Microsoft.AspNetCore.Authentication.OAuth;
using Refit;
using System.Reflection;
using Mapster;
using MapsterMapper;
using System.Linq.Expressions;
using ExpressionDebugger;
using AirTravelAggregatorAPI.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace AirTravelAggregatorAPI
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient<IFlightAggregatorService, FlightAggregateService>();
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != null && environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                services.AddTransient<IFirstFlightService, FirstFlightTestService>();
                services.AddTransient<ISecondFlightService, SecondFlightTestService>();
            }
            else
            {
                services.AddRefitEndpoints(configuration);
            }
            services.AddSingleton(GetConfigureMappinfConfig());
            services.AddScoped<IMapper, ServiceMapper>();
            
           
            //MapsterConfiguration.Configure();
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
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
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            };

            var securityReq = new OpenApiSecurityRequirement()
            {
                {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }};


            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1"
                });
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });
            return services;
        }
    }
}
