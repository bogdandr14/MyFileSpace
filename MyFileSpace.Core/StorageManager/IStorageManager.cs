using Microsoft.AspNetCore.Http;

namespace MyFileSpace.Core.StorageManager
{
    public interface IStorageManager
    {
        Task<byte[]> ReadFile(string directory, string fileName);
        Task UploadFile(string directory, string fileName, IFormFile file);
        Task<bool> RemoveFile(string directory, string fileName);
        Task AddDirectory(string directoryName);
    }
}
