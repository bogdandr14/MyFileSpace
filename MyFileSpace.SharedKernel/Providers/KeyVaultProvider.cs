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
            string secretKey = _configuration.GetConfigValue(secretName);
            var secret = await _secretClient.GetSecretAsync(secretKey);

            if (secret.GetRawResponse().IsError)
            {
                throw new Exception($"Unable to retrieve secret {secretName}!");
            };

            return secret.Value.Value;
        }
    }
}
