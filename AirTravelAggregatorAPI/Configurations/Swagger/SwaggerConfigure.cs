using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AirTravelAggregatorAPI.Configurations.Swagger
{
    public static class SwaggerConfigure
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
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
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                o.IncludeXmlComments(xmlPath);
                o.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);
                o.DocumentFilter<EnumTypesDocumentFilter>();
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
