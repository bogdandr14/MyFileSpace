using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Infrastructure.Helpers;

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

        public byte[] ReadFromFileSystem(string storedFileName)
        {
            string fullPath = GetFullPath(storedFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File could not be found");
            }

            return File.ReadAllBytes(fullPath);
        }

        public void UpdateInFileSystem(string storedFileName, IFormFile file)
        {
            if (!RemoveFromFileSystem(storedFileName))
            {
                throw new Exception("File not found to update");
            }

            AddInFileSystem(storedFileName, file);
        }

        public bool RemoveFromFileSystem(string storedFileName)
        {
            string fullPath = GetFullPath(storedFileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public void AddInFileSystem(string storedFileName, IFormFile file)
        {
            using (FileStream fileStream = File.Create(GetFullPath(storedFileName)))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                file.OpenReadStream().CopyToAsync(fileStream);
            }
        }

        private string GetFullPath(string storedFileName)
        {
            return Path.Combine(_fileDirectoryPath, storedFileName);
        }
    }
}
