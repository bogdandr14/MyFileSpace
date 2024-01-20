using Microsoft.AspNetCore.Http;
using MyFileSpace.Api.Extensions;
using MyFileSpace.Core.Helpers;
using MyFileSpace.SharedKernel.DTOs;
using MyFileSpace.SharedKernel.Repositories;

namespace MyFileSpace.Core.Services
{
    internal class FileManagementService : IFileManagementService
    {
        private readonly IFileDataRepository _fileDataRepository;
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly ICacheRepository _cacheRepository;

        public FileManagementService(IFileDataRepository fileDataRepository, IFileSystemRepository fileSystemRepository, ICacheRepository cacheRepository)
        {
            _fileDataRepository = fileDataRepository;
            _fileSystemRepository = fileSystemRepository;
            _cacheRepository = cacheRepository;
        }

        public IEnumerable<string> GetAllFileNames()
        {
            return _cacheRepository.GetAndSet(CacheKeys.ALL_FILES, () => _fileDataRepository.GetAll().Select(x => x.OriginalName).ToList());
        }

        public FileDTO GetFileData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception("No filename given");
            }

            return _cacheRepository.GetAndSet($"{CacheKeys.FILE_DATA_PREFIX}{fileName}", () => RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName)));
        }

        public async Task<byte[]> GetFileByName(string fileName)
        {
            FileDTO fileObject = _cacheRepository.GetAndSet($"{CacheKeys.FILE_DATA_PREFIX}{fileName}", () => RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName)));
            Func<byte[]> fileBytesFunct = () => _fileSystemRepository.ReadFromFileSystem(fileObject.StoredFileName()).GetAwaiter().GetResult();
            return await Task.FromResult(_cacheRepository.GetAndSetBytes($"{CacheKeys.FILE_BYTES_PREFIX}{fileName}", fileBytesFunct, TimeSpan.FromMinutes(1)));
        }

        public async Task AddFile(IFormFile file)
        {
            FileDTO fileData = file.CreateNewFileDTO();
            if (_fileDataRepository.GetByName(file.FileName) != null)
            {
                throw new Exception($"File with name ${file.FileName} already exists!");
            }

            await _fileSystemRepository.AddInFileSystem(fileData.StoredFileName(), file);
            _fileDataRepository.Add(fileData);

            _cacheRepository.Remove($"{CacheKeys.ALL_FILES}");
            _cacheRepository.Set($"{CacheKeys.FILE_DATA_PREFIX}{fileData.OriginalName}", fileData);
        }

        public async Task UpdateFile(Guid fileGuid, IFormFile file)
        {
            FileDTO existingFile = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));

            FileDTO? fileWithSameName = _fileDataRepository.GetByName(file.FileName);
            if (fileWithSameName != null && fileWithSameName.Guid.Equals(existingFile.Guid))
            {
                throw new Exception($"Can not rename file ${existingFile.OriginalName} to ${file.FileName} as it already exists!");
            }

            _cacheRepository.Remove($"{CacheKeys.FILE_DATA_PREFIX}{existingFile.OriginalName}");
            _cacheRepository.Remove($"{CacheKeys.FILE_BYTES_PREFIX}{existingFile.OriginalName}");

            FileDTO updatedFileData = file.UpdateExistingFileDTO(existingFile);
            await _fileSystemRepository.UpdateInFileSystem(updatedFileData.StoredFileName(), file);
            _fileDataRepository.Update(updatedFileData);

            _cacheRepository.Remove($"{CacheKeys.ALL_FILES}");
            _cacheRepository.Set($"{CacheKeys.FILE_DATA_PREFIX}{updatedFileData.OriginalName}", updatedFileData);
        }

        public async Task DeleteFile(Guid fileGuid)
        {
            FileDTO fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            bool isFileDeleted = await _fileSystemRepository.RemoveFromFileSystem(fileObject.StoredFileName());
            if (!isFileDeleted)
            {
                throw new Exception("Could not delete file");
            }

            _fileDataRepository.Delete(fileGuid);

            _cacheRepository.Remove($"{CacheKeys.ALL_FILES}");
            _cacheRepository.Remove($"{CacheKeys.FILE_DATA_PREFIX}{fileObject.OriginalName}");
            _cacheRepository.Remove($"{CacheKeys.FILE_BYTES_PREFIX}{fileObject.OriginalName}");
        }

        private FileDTO RetrieveValidFileData(Func<FileDTO?> func)
        {
            FileDTO? fileObject = func.Invoke();

            if (fileObject is null)
            {
                throw new Exception("File not found");
            }

            return fileObject;
        }
    }
}

