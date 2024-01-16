using Microsoft.Extensions.Configuration;

namespace MyFileStorage.Core.Providers
{
    internal class AppConfigurationProvider : IAppConfigurationProvider
    {
        private readonly IConfiguration _configuration;
        public AppConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfigurationSection GetSection(string configurationName)
        {
            IConfigurationSection configurationSection = _configuration.GetSection(configurationName);
            if (configurationSection == null)
            {
                throw new Exception($"Configuration section {configurationName} not found");
            }

            return _configuration.GetSection(configurationName);
        }

        public string GetValue(string configurationName)
        {
            string? configurationValue = GetSection(configurationName).Value;
            if (configurationValue == null)
            {
                throw new Exception($"Value not found for configuration section {configurationName}");
            }

            return configurationValue;
        }
    }
}
