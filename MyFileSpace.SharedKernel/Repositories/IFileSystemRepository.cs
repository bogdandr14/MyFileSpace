using Microsoft.AspNetCore.Http;

namespace MyFileSpace.SharedKernel.Repositories
{
    public interface IFileSystemRepository
    {
        Task<byte[]> ReadFromFileSystem(string fileName);
        Task AddInFileSystem(string fileName, IFormFile file);
        Task UpdateInFileSystem(string fileName, IFormFile file);
        Task<bool> RemoveFromFileSystem(string fileName);
    }
}
