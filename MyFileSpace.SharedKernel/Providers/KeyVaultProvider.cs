using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace MyFileSpace.SharedKernel.Providers
{
    internal class KeyVaultProvider : ISecretProvider
    {
        private readonly SecretClient _secretClient;
        private readonly IConfiguration _configuration;

        public KeyVaultProvider(SecretClient secretClient, IConfiguration configuration)
        {
            _secretClient = secretClient;
            _configuration = configuration;
        }

        public async Task<string> GetSecret(string secretName)
        {
            var secret = await _secretClient.GetSecretAsync(_configuration.GetConfigValue(secretName));

            if (secret.GetRawResponse().IsError)
            {
                throw new Exception($"Unable to retrieve secret {secretName}!");
            };

            return secret.Value.Value;
        }
    }
}
