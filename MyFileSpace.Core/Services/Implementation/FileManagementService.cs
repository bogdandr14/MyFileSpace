using Microsoft.AspNetCore.Http;
using MyFileSpace.Api.Extensions;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.DTOs;
using MyFileSpace.SharedKernel.Exceptions;

namespace MyFileSpace.Core.Services.Implementation
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

        #region "Public methods"
        public async Task<IEnumerable<string>> GetAllFileNames()
        {
            return await _cacheRepository.GetAndSetAsync(CacheKeys.ALL_FILES, () => _fileDataRepository.GetAll().Select(x => x.OriginalName).ToList());
        }

        public async Task<FileDTO_old> GetFileData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new InvalidException("No filename given");
            }

            return await _cacheRepository.GetAndSetAsync($"{CacheKeys.FILE_DATA_PREFIX}{fileName}", () => RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName)));
        }

        public async Task<byte[]> GetFileByName(string fileName)
        {
            FileDTO_old fileObject = await _cacheRepository.GetAndSetAsync($"{CacheKeys.FILE_DATA_PREFIX}{fileName}", () => RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName)));
            Func<byte[]> fileBytesFunct = () => _fileSystemRepository.ReadFromFileSystem(fileObject.StoredFileName()).GetAwaiter().GetResult();
            return await _cacheRepository.GetAndSetBytesAsync($"{CacheKeys.FILE_BYTES_PREFIX}{fileName}", fileBytesFunct, TimeSpan.FromMinutes(1));
        }

        public async Task AddFile(IFormFile file)
        {
            FileDTO_old fileData = file.CreateNewFileDTO();
            if (_fileDataRepository.GetByName(file.FileName) != null)
            {
                throw new InvalidException($"File with name ${file.FileName} already exists!");
            }

            await _fileSystemRepository.AddInFileSystem(fileData.StoredFileName(), file);
            _fileDataRepository.Add(fileData);

            await _cacheRepository.RemoveAsync($"{CacheKeys.ALL_FILES}");
            await _cacheRepository.SetAsync($"{CacheKeys.FILE_DATA_PREFIX}{fileData.OriginalName}", fileData);
        }

        public async Task UpdateFile(Guid fileGuid, IFormFile file)
        {
            FileDTO_old existingFile = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));

            FileDTO_old? fileWithSameName = _fileDataRepository.GetByName(file.FileName);
            if (fileWithSameName != null && fileWithSameName.Guid.Equals(existingFile.Guid))
            {
                throw new InvalidException($"Can not rename file ${existingFile.OriginalName} to ${file.FileName} as it already exists!");
            }

            await _cacheRepository.RemoveAsync($"{CacheKeys.FILE_DATA_PREFIX}{existingFile.OriginalName}");
            await _cacheRepository.RemoveAsync($"{CacheKeys.FILE_BYTES_PREFIX}{existingFile.OriginalName}");

            FileDTO_old updatedFileData = file.UpdateExistingFileDTO(existingFile);
            await _fileSystemRepository.UpdateInFileSystem(updatedFileData.StoredFileName(), file);
            _fileDataRepository.Update(updatedFileData);

            await _cacheRepository.RemoveAsync($"{CacheKeys.ALL_FILES}");
            await _cacheRepository.SetAsync($"{CacheKeys.FILE_DATA_PREFIX}{updatedFileData.OriginalName}", updatedFileData);
        }

        public async Task DeleteFile(Guid fileGuid)
        {
            FileDTO_old fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            bool isFileDeleted = await _fileSystemRepository.RemoveFromFileSystem(fileObject.StoredFileName());
            if (!isFileDeleted)
            {
                throw new InvalidException("Could not delete file");
            }

            _fileDataRepository.Delete(fileGuid);

            await _cacheRepository.RemoveAsync($"{CacheKeys.ALL_FILES}");
            await _cacheRepository.RemoveAsync($"{CacheKeys.FILE_DATA_PREFIX}{fileObject.OriginalName}");
            await _cacheRepository.RemoveAsync($"{CacheKeys.FILE_BYTES_PREFIX}{fileObject.OriginalName}");
        }
        #endregion

        private FileDTO_old RetrieveValidFileData(Func<FileDTO_old?> func)
        {
            FileDTO_old? fileObject = func.Invoke();

            if (fileObject is null)
            {
                throw new NotFoundException("File not found");
            }

            return fileObject;
        }
    }
}

