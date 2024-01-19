using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Infrastructure.Helpers;
using MyFileSpace.SharedKernel.Repositories;

namespace MyFileSpace.Infrastructure.Repositories
{
    internal class FileSystemRepository : IFileSystemRepository
    {
        private readonly string _fileDirectoryPath;

        public FileSystemRepository(IConfiguration configuration)
        {
            _fileDirectoryPath = configuration.GetConfigValue("FilesDirectoryPath");
            Directory.CreateDirectory(_fileDirectoryPath);
        }

        public async Task<byte[]> ReadFromFileSystem(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File could not be found");
            }
            byte[] result;
            using (FileStream fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read))
            {
                result = new byte[fileStream.Length];
                await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
            }

            Task<byte[]> task = File.ReadAllBytesAsync(fullPath);
            byte[] bytes = await task;
            return bytes;
        }

        public async Task UpdateInFileSystem(string fileName, IFormFile file)
        {
            if (!RemoveFromFileSystem(fileName).Result)
            {
                throw new Exception("File not found to update");
            }

            AddInFileSystem(fileName, file);
        }

        public async Task<bool> RemoveFromFileSystem(string fileName)
        {
            string fullPath = GetFullPath(fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public async Task AddInFileSystem(string fileName, IFormFile file)
        {
            using (FileStream fileStream = File.Create(GetFullPath(fileName)))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                await file.OpenReadStream().CopyToAsync(fileStream);
            }
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(_fileDirectoryPath, fileName);
        }
    }
}
