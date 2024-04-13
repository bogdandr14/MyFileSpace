using Microsoft.AspNetCore.Http;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class AzureStorageRepository : IFileStorageRepository
    {
        public Task AddFileInFileStorage(string fileName, IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadFileFromFileStorage(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveFileFromFileStorage(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateFileInFileStorage(string fileName, IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
