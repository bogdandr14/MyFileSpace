using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using MyFileSpace.Api.Middlewares;
using MyFileSpace.Api.Providers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyFileSpace.Api
{
    internal static class ApiModule
    {
        internal static void RegisterApiServices(this IServiceCollection services, bool isDevelopment)
        {
            services.AddTransient<CustomExceptionHandlerMiddleware>();
            services.AddSingleton<IHttpContextProvider, HttpContextProvider>();

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddSwaggerConfiguration();
            services.AddServiceDescriptorConfiguration();
        }

        // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
        private static void AddServiceDescriptorConfiguration(this IServiceCollection services)
        {
            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);

                // optional - default path to view services is /listallservices - recommended to choose your own path
                config.Path = Constants.SERVICES_LIST_PATH;
            });
        }

        private static void AddSwaggerConfiguration(this IServiceCollection serviceCollection)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddSwaggerGen(options =>
            {
                options.AddSwaggerDoc();

                options.AddSwaggerSecurityDefinition();

                options.AddSwaggerSecurityRequirement();
            });
        }

        private static void AddSwaggerDoc(this SwaggerGenOptions options)
        {
            options.SwaggerDoc(Constants.VERSION, new OpenApiInfo
            {
                Title = Constants.TITLE,
                Version = Constants.VERSION,
                Description = Constants.DESCRIPTION
            });
        }

        private static void AddSwaggerSecurityDefinition(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition(Constants.SWAGGER_SECURITY_NAME, new OpenApiSecurityScheme
            {
                Name = Constants.SWAGGER_AUTHORIZATION,
                Type = SecuritySchemeType.Http,
                Scheme = OpenApiConstants.Bearer,
                BearerFormat = OpenApiConstants.Jwt,
                In = ParameterLocation.Header,
                Description = Constants.SWAGGER_SECURITY_DESCRIPTION
            });
        }

        private static void AddSwaggerSecurityRequirement(this SwaggerGenOptions options)
        {
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = Constants.SWAGGER_SECURITY_NAME
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}
