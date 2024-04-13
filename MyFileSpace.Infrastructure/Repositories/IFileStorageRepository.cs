using Microsoft.AspNetCore.Http;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IFileStorageRepository
    {
        Task<byte[]> ReadFileFromFileStorage(string fileName);
        Task AddFileInFileStorage(string fileName, IFormFile file);
        Task UpdateFileInFileStorage(string fileName, IFormFile file);
        Task<bool> RemoveFileFromFileStorage(string fileName);
    }
}
