using Microsoft.Extensions.Configuration;

namespace MyFileSpace.SharedKernel.Helpers
{
    public static class AppConfigurationProvider
    {
        public static IConfigurationSection GetConfigSection(this IConfiguration configuration, string configurationName)
        {
            IConfigurationSection configurationSection = configuration.GetSection(configurationName);
            if (configurationSection == null)
            {
                throw new Exception($"Configuration section {configurationName} not found in config file");
            }

            return configurationSection;
        }

        public static string GetConfigValue(this IConfiguration configuration, string configurationName)
        {
            string? configurationValue = configuration.GetSection(configurationName).Value;
            if (configurationValue == null)
            {
                throw new Exception($"Property {configurationName} not found in config file at path {configuration.GetSection(configurationName).Path}");
            }

            return configurationValue;
        }
    }
}
