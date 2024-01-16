using Microsoft.Extensions.Configuration;

namespace MyFileSpace.Core.Providers
{
    public interface IAppConfigurationProvider
    {
        IConfigurationSection GetSection(string configurationName);
        string GetValue(string configurationName);
    }
}
