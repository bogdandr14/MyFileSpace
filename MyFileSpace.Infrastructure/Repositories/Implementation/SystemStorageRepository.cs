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
            _fileDirectoryPath = configuration.GetConfigValue("FilesDirectoryPath");
            _fileEncryptionKey = secretProvider.GetSecret("FileEncryptionKey").GetAwaiter().GetResult();
            Directory.CreateDirectory(_fileDirectoryPath);
        }

        public async Task<byte[]> ReadFileFromFileStorage(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (!File.Exists(fullPath))
            {
                throw new NotFoundException("File could not be found");
            }

            using (FileStream fileStream = File.OpenRead(fullPath))
            {
                return await CryptographyUtility.DecryptAsync(fileStream, _fileEncryptionKey);
            }
        }

        public async Task UpdateFileInFileStorage(string fileName, IFormFile file)
        {
            if (!RemoveFileFromFileStorage(fileName).Result)
            {
                throw new NotFoundException("File could not be found in the file system");
            }

            await AddFileInFileStorage(fileName, file);
        }

        public async Task<bool> RemoveFileFromFileStorage(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task AddFileInFileStorage(string fileName, IFormFile file)
        {
            using (FileStream fileStream = File.Create(GetFullPath(fileName)))
            {
                await CryptographyUtility.EncryptAsync(file.OpenReadStream(), _fileEncryptionKey, fileStream);
            }
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(_fileDirectoryPath, fileName);
        }
    }
}
