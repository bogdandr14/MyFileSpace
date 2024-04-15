using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;
using MyFileSpace.SharedKernel.Providers;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class SystemStorageRepository : IFileStorageRepository
    {
        private readonly string _fileDirectoryPath;
        private readonly string _fileEncryptionKey;

        public SystemStorageRepository(IConfiguration configuration, ISecretProvider secretProvider)
        {
            _fileDirectoryPath = configuration.GetConfigValue("FileStorage:DirectoryPath");
            _fileEncryptionKey = secretProvider.GetSecret("FileStorage:EncryptionKey").GetAwaiter().GetResult();
            Directory.CreateDirectory(_fileDirectoryPath);
        }

        public async Task<Stream> ReadFile(string directory, string fileName)
        {
            string fullPath = GetFullPath(directory, fileName);

            if (!File.Exists(fullPath))
            {
                throw new NotFoundException("File could not be found");
            }

            using (FileStream fileStream = File.OpenRead(fullPath))
            {
                return await CryptographyUtility.DecryptAsync(fileStream, _fileEncryptionKey);
            }
        }

        public async Task<bool> RemoveFile(string directory, string fileName)
        {
            string fullPath = GetFullPath(directory, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task UploadFile(string directory, string fileName, IFormFile file)
        {
            string fullPath = GetFullPath(directory, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            using (FileStream fileStream = File.Create(fullPath))
            {
                await CryptographyUtility.EncryptAsync(file.OpenReadStream(), _fileEncryptionKey, fileStream);
            }
        }

        public Task AddDirectory(string directoryName)
        {
            return Task.FromResult(Directory.CreateDirectory(Path.Combine(_fileDirectoryPath, directoryName)));
        }

        private string GetFullPath(string directory, string fileName)
        {
            return Path.Combine(_fileDirectoryPath, directory, fileName);
        }
    }
}
