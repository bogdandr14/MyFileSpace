using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class FileSystemRepository : IFileSystemRepository
    {
        private readonly string _fileDirectoryPath;
        private readonly string CRYPTO_PASSPHRASE = "a";

        public FileSystemRepository(IConfiguration configuration)
        {
            _fileDirectoryPath = configuration.GetConfigValue("FilesDirectoryPath");
            Directory.CreateDirectory(_fileDirectoryPath);
        }

        public async Task<byte[]> ReadFileFromFileSystem(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (!File.Exists(fullPath))
            {
                throw new NotFoundException("File could not be found");
            }

            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task<byte[]> ReadDecryptedFileFromFileSystem(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (!File.Exists(fullPath))
            {
                throw new NotFoundException("File could not be found");
            }

            using FileStream fileStream = File.OpenRead(fullPath);
            var encryptedBytes = GetBytesFromStream(fileStream);
            //var decryptedBytes = await CryptographyUtility.DecryptAsync(encryptedBytes, CRYPTO_PASSPHRASE);
            //return decryptedBytes;
            return encryptedBytes;
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
                fileStream.Seek(0, SeekOrigin.Begin);
                await file.OpenReadStream().CopyToAsync(fileStream);
            }
        }

        public async Task AddEncryptedFileInFileSystem(string fileName, IFormFile file)
        {
            using (FileStream fileStream = File.Create(GetFullPath(fileName)))
            {
                var unencryptedBytes = GetBytesFromStream(file.OpenReadStream());
                //byte[] encryptedBytes = await CryptographyUtility.EncryptAsync(unencryptedBytes, CRYPTO_PASSPHRASE);
                fileStream.Seek(0, SeekOrigin.Begin);
                //fileStream.Write(encryptedBytes);
                fileStream.Write(unencryptedBytes);
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

        private byte[] GetBytesFromStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
