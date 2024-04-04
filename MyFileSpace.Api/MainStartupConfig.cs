using Ardalis.ListStartupServices;
using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using MyFileSpace.Api.Filters;
using MyFileSpace.Api.Providers;
using MyFileSpace.Core;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
namespace MyFileSpace.Api
{
    public static class MainStartupConfig
    {
        #region "Builder setup"
        // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
        public static void AddServiceDescriptorConfiguration(this IServiceCollection services)
        {
            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);

                // optional - default path to view services is /listallservices - recommended to choose your own path
                config.Path = Constants.SERVICES_LIST_PATH;
            });
        }

        public static void AddModulesConfiguration(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            bool isDevelopment = environment.EnvironmentName.Equals(Constants.DEVELOPMENT, StringComparison.OrdinalIgnoreCase);
            services.RegisterDbContext(configuration);
            
            services.RegisterInfrastructureServices(isDevelopment);
            services.RegisterCoreServices(isDevelopment);
            services.AddScoped<CustomExceptionFilterAttribute>();
            services.AddSingleton<IHttpContextProvider, HttpContextProvider>();
            services.AddScoped<Session>();

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });
        }

        public static void AddSwaggerConfiguration(this IServiceCollection serviceCollection)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddSwaggerGen(options =>
            {
                options.AddSwaggerDoc();

                options.AddSwaggerSecurityDefinition();

                options.AddSwaggerSecurityRequirement();

                //options.AddSwaggerSummary();

                //options.EnableAnnotations();

                //options.OperationFilter<FastEndpointsOperationFilter>();
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

            options.AddSecurityDefinition(Constants.AUTH_R_HEADER, new OpenApiSecurityScheme
            {
                Name = Constants.AUTH_R_HEADER,
                Type = SecuritySchemeType.Http,
                Scheme = OpenApiConstants.Schema,
                In = ParameterLocation.Header,
                Description = Constants.AUTH_R_HEADER
            });

            options.AddSecurityDefinition(Constants.AUTH_N_HEADER, new OpenApiSecurityScheme
            {
                Name = Constants.AUTH_N_HEADER,
                Type = SecuritySchemeType.Http,
                Scheme = OpenApiConstants.Headers,
                In = ParameterLocation.Header,
                Description = Constants.AUTH_N_HEADER
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

        private static void AddSwaggerSummary(this SwaggerGenOptions options)
        {
            // add summaries to endpoints
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        }
        #endregion

        #region "Application setup"
        public static void UseSwaggerUIConfiguration(this IApplicationBuilder builder)
        {
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            builder.UseSwaggerUI();
        }
        public static void UserCorsConfiguration(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        }

        #endregion
    }

}