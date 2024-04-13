using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
            services.AddSingleton(provider =>
                    new SecretClient(new Uri(configuration.GetConfigValue("AzureKeyVaultUri")), new DefaultAzureCredential()));
            services.AddSingleton<ISecretProvider, KeyVaultProvider>();
        }
    }
}
