using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Api.Extensions;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Core.Services
{
    internal class FileManagementService : IFileManagementService
    {
        private readonly IFileDataRepository _fileDataRepository;
        private readonly IFileSystemRepository _fileSystemRepository;

        public FileManagementService(IFileDataRepository fileDataRepository, IFileSystemRepository fileSystemRepository, IConfiguration configuration)
        {
            _fileDataRepository = fileDataRepository;
            _fileSystemRepository = fileSystemRepository;
        }

        public IEnumerable<string> GetAllFileNames()
        {
            return _fileDataRepository.GetAll().Select(x => x.OriginalName).ToList();
        }

        public FileData GetFileData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception("No filename given");
            }

            return RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName));
        }

        public byte[] GetFileByGuid(Guid fileGuid)
        {
            FileData fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            return _fileSystemRepository.ReadFromFileSystem(fileObject.StoredName);
        }

        public void AddFile(IFormFile file)
        {
            FileData fileData = file.ToFileData();
            _fileSystemRepository.AddInFileSystem(fileData.StoredName, file);
            _fileDataRepository.Add(fileData);
        }

        public void UpdateFile(Guid fileGuid, IFormFile file)
        {
            FileData fileData = file.ToFileData();
            _fileSystemRepository.UpdateInFileSystem(fileData.StoredName, file);
            _fileDataRepository.Update(file.ToFileData());
        }

        public void DeleteFile(Guid fileGuid)
        {
            FileData fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            if (!_fileSystemRepository.RemoveFromFileSystem(fileObject.StoredName))
            {
                throw new Exception("Could not delete file");
            }
            _fileDataRepository.Delete(fileGuid);
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

