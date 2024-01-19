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
            return _cacheRepository.GetAndSet(CacheHelper.ALL_FILES, () => _fileDataRepository.GetAll().Select(x => x.OriginalName).ToList());
        }

        public FileData GetFileData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception("No filename given");
            }

            return _cacheRepository.GetAndSet($"{CacheHelper.FILE_DATA_PREFIX}{fileName}", () => RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName)));
        }

        public Task<byte[]> GetFileByName(string fileName)
        {
            FileData fileObject = _cacheRepository.GetAndSet($"{CacheHelper.FILE_DATA_PREFIX}{fileName}", () => RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName)));
            return _cacheRepository.GetAndSet($"{CacheHelper.FILE_DATA_PREFIX}{fileName}", () => _fileSystemRepository.ReadFromFileSystem(fileObject.StoredFileName()), TimeSpan.FromMinutes(1));
        }

        public async Task AddFile(IFormFile file)
        {
            FileData fileData = file.NewFileData();
            if (_fileDataRepository.GetByName(file.FileName) != null)
            {
                throw new Exception($"File with name ${file.FileName} already exists!");
            }

            await _fileSystemRepository.AddInFileSystem(fileData.StoredFileName(), file);
            _fileDataRepository.Add(fileData);

            _cacheRepository.Remove($"{CacheHelper.ALL_FILES}");
            _cacheRepository.Set($"{CacheHelper.FILE_DATA_PREFIX}{fileData.OriginalName}", fileData);
        }

        public async Task UpdateFile(Guid fileGuid, IFormFile file)
        {
            FileData fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            if (!fileObject.OriginalName.Split('.').Last().Equals(file.FileName.Split('.').Last()))
            {
                throw new Exception("Incorrect file format");
            }

            FileData? fileWithSameName = _fileDataRepository.GetByName(file.FileName);
            if (fileWithSameName != null! && fileWithSameName.Guid.Equals(fileObject.Guid))
            {
                throw new Exception($"Can not rename file ${fileObject.OriginalName} to ${file.FileName} as it already exists!");
            }

            await _fileSystemRepository.UpdateInFileSystem(fileObject.StoredFileName(), file);
            FileData updatedFileData = file.ExistingFileData(fileGuid);
            _fileDataRepository.Update(updatedFileData);

            _cacheRepository.Remove($"{CacheHelper.ALL_FILES}");
            _cacheRepository.Remove($"{CacheHelper.FILE_DATA_PREFIX}{fileObject.OriginalName}");
            _cacheRepository.Set($"{CacheHelper.FILE_DATA_PREFIX}{updatedFileData.OriginalName}", updatedFileData);
        }

        public async Task DeleteFile(Guid fileGuid)
        {
            FileData fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            bool isFileDeleted = await _fileSystemRepository.RemoveFromFileSystem(fileObject.StoredFileName());
            if (!isFileDeleted)
            {
                throw new Exception("Could not delete file");
            }

            _fileDataRepository.Delete(fileGuid);

            _cacheRepository.Remove($"{CacheHelper.ALL_FILES}");
            _cacheRepository.Remove($"{CacheHelper.FILE_DATA_PREFIX}{fileObject.OriginalName}");
        }

        private FileData RetrieveValidFileData(Func<FileData?> func)
        {
            FileData? fileObject = func.Invoke();

            if (fileObject is null)
            {
                throw new Exception("File not found");
            }

            return fileObject;
        }
    }
}

