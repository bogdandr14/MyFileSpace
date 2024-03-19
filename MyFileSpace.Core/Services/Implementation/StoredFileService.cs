using MyFileSpace.Infrastructure.Repositories;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class StoredFileService
    {
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly ICacheRepository _cacheRepository;

        public StoredFileService(IStoredFileRepository storedFileRepository, IFileSystemRepository fileSystemRepository, ICacheRepository cacheRepository)
        {
            _storedFileRepository = storedFileRepository;
            _fileSystemRepository = fileSystemRepository;
            _cacheRepository = cacheRepository;
        }
    }
}
