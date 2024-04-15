using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFileSpace.SharedKernel.Providers;

namespace MyFileSpace.SharedKernel
{
    public static class SharedKernelModule
    {
        public static void RegisterSharedKernelServices(this IServiceCollection services, bool isDevelopment, IConfiguration configuration)
        {
            if (isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(services);
            }
            else
            {
                RegisterProductionOnlyDependencies(services, configuration);
            }
        }

        private static void RegisterDevelopmentOnlyDependencies(IServiceCollection services)
        {
            services.AddSingleton<ISecretProvider, DefaultSecretProvider>();
        }

        private static void RegisterProductionOnlyDependencies(IServiceCollection services, IConfiguration configuration)
        {
            string tenantId = configuration.GetConfigValue("AzureKeyVault:TenantId");
            string clientId = configuration.GetConfigValue("AzureKeyVault:ClientId");
            string clientSecretId = configuration.GetConfigValue("AzureKeyVault:ClientSecretId");
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecretId);
            services.AddSingleton(provider =>
                    new SecretClient(new Uri(configuration.GetConfigValue("AzureKeyVault:Uri")), clientSecretCredential));
            services.AddSingleton<ISecretProvider, KeyVaultProvider>();
        }
    }
}
