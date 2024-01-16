using Microsoft.Extensions.Configuration;

namespace MyFileStorage.Core.Providers
{
    public interface IAppConfigurationProvider
    {
        IConfigurationSection GetSection(string configurationName);
        string GetValue(string configurationName);
    }
}
