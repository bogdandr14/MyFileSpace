using Microsoft.AspNetCore.Http;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IFileStorageRepository
    {
        Task<byte[]> ReadFile(string directory, string fileName);
        Task UploadFile(string directory, string fileName, IFormFile file);
        Task<bool> RemoveFile(string directory, string fileName);
        Task AddDirectory(string directoryName);
    }
}
