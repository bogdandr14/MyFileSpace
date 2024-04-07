using Microsoft.AspNetCore.Http;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IFileSystemRepository
    {
        Task<byte[]> ReadFileFromFileSystem(string fileName);
        Task<byte[]> ReadDecryptedFileFromFileSystem(string fileName);
        Task AddFileInFileSystem(string fileName, IFormFile file);
        Task AddEncryptedFileInFileSystem(string fileName, IFormFile file);
        Task UpdateFileInFileSystem(string fileName, IFormFile file);
        Task<bool> RemoveFileFromFileSystem(string fileName);
        Task AddDirectoryInFileSystem(string directoryName);
    }
}
