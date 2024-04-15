using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class FileSystemRepository : IFileSystemRepository
    {
        private readonly string _fileDirectoryPath;
        private readonly string _fileEncryptionKey;

        public FileSystemRepository(IConfiguration configuration)
        {
            _fileDirectoryPath = configuration.GetConfigValue("FilesDirectoryPath");
            _fileEncryptionKey = configuration.GetConfigValue("FileEncryptionKey");
            Directory.CreateDirectory(_fileDirectoryPath);
        }

        public async Task<byte[]> ReadFileFromFileSystem(string fileName)
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

        public async Task UpdateFileInFileSystem(string fileName, IFormFile file)
        {
            if (!RemoveFileFromFileSystem(fileName).Result)
            {
                throw new NotFoundException("File could not be found in the file system");
            }

            await AddFileInFileSystem(fileName, file);
        }

        public async Task<bool> RemoveFileFromFileSystem(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task AddFileInFileSystem(string fileName, IFormFile file)
        {
            using (FileStream fileStream = File.Create(GetFullPath(fileName)))
            {
                await CryptographyUtility.EncryptAsync(file.OpenReadStream(), _fileEncryptionKey, fileStream);
            }
        }

        public Task AddDirectoryInFileSystem(string directoryName)
        {
            Directory.CreateDirectory(Path.Combine(_fileDirectoryPath, directoryName));
            return Task.CompletedTask;
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(_fileDirectoryPath, fileName);
        }
    }
}
