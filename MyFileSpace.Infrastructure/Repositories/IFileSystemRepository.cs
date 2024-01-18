using Microsoft.AspNetCore.Http;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IFileSystemRepository
    {
        byte[] ReadFromFileSystem(string storedFileName);
        void AddInFileSystem(string storedFileName, IFormFile file);
        void UpdateInFileSystem(string storedFileName, IFormFile file);
        bool RemoveFromFileSystem(string storedFileName);
    }
}
