using Azure.Core;
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
        internal static void ConfigureKeyVault(this IHostApplicationBuilder hostApplicationBuilder)
        {
            string keyVaultEndpoint = GetEnvironmentVariable("KEYVAULT_ENDPOINT");
            SecretClient secretClient = new SecretClient(new Uri(keyVaultEndpoint), hostApplicationBuilder.GetTokenCredential());
            hostApplicationBuilder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
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
        internal static void UserCorsConfiguration(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        }
        #endregion

        #region "Helpers"
        private static TokenCredential GetTokenCredential(this IHostApplicationBuilder hostApplicationBuilder)
        {
            if (hostApplicationBuilder.Environment.IsProduction())
            {
                return new DefaultAzureCredential();
            }

            string tenantId = GetEnvironmentVariable("KEYVAULT_TENANTID");
            string clientId = GetEnvironmentVariable("KEYVAULT_CLIENTID");
            string clientSecret = GetEnvironmentVariable("KEYVAULT_CLIENTSECRET");

            return new ClientSecretCredential(tenantId, clientId, clientSecret);
        }

        private static string GetEnvironmentVariable(string key)
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