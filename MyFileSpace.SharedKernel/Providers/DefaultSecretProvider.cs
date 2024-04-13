using Microsoft.Extensions.Configuration;

namespace MyFileSpace.SharedKernel.Providers
{
    internal class DefaultSecretProvider : ISecretProvider
    {
        private readonly IConfiguration _configuration;

        public DefaultSecretProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetSecret(string secretName)
        {
            return await Task.FromResult(_configuration.GetConfigValue(secretName));
        }
    }
}
