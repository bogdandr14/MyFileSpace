using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MyFileSpace.Core;
using MyFileSpace.Infrastructure;
using MyFileSpace.Infrastructure.Persistence;
namespace MyFileSpace.Api
{
    internal static class MainStartupConfig
    {
        #region "Builder setup"

        public static void ConfigureMyFileSpaceConfiguration(this WebApplicationBuilder configureHostBuilder)
        {
            if (configureHostBuilder.Environment.IsProduction())
            {
                configureHostBuilder.Configuration.ConfigureKeyVault();
            }
        }

        internal static void AddModulesConfiguration(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            bool isDevelopment = environment.IsDevelopment();

            services.RegisterDbContext(configuration);
            services.RegisterInfrastructureServices(isDevelopment, configuration);
            services.RegisterCoreServices();
            services.RegisterApiServices(isDevelopment);
        }
        #endregion

        #region "Application setup"
        internal static void UseSwaggerUIConfiguration(this IApplicationBuilder builder)
        {
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            builder.UseSwaggerUI();
        }
        internal static void UserCorsConfiguration(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        }

        private static void ConfigureKeyVault(this IConfigurationBuilder builder)
        {
            string keyVaultEndpoint = RetrieveEnvironmentVariable("KEYVAULT_ENDPOINT");
            string tenantId = RetrieveEnvironmentVariable("KEYVAULT_TENANTID");
            string clientId = RetrieveEnvironmentVariable("KEYVAULT_CLIENTID");
            string clientSecret = RetrieveEnvironmentVariable("KEYVAULT_CLIENTSECRET");

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            SecretClient secretClient = new SecretClient(new Uri(keyVaultEndpoint), clientSecretCredential);

            builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        }

        private static string RetrieveEnvironmentVariable(string key)
        {
            string? value = Environment.GetEnvironmentVariable(key);
            if (value is null)
            {
                throw new InvalidOperationException($"The environment variable ${key} was not set");
            }

            return value;
        }
        #endregion
    }

}