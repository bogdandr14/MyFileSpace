using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Api.Extensions;
using MyFileSpace.SharedKernel.Repositories;
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

        public Task<byte[]> GetFileByName(string fileName)
        {
            FileData fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByName(fileName));
            return _fileSystemRepository.ReadFromFileSystem(fileObject.StoredFileName());
        }

        public async Task AddFile(IFormFile file)
        {
            FileData fileData = file.NewFileData();
            if(_fileDataRepository.GetByName(file.FileName)!= null)
            {
                throw new Exception($"File with name ${file.FileName} already exists!");
            }

            await _fileSystemRepository.AddInFileSystem(fileData.StoredFileName(), file);
            _fileDataRepository.Add(fileData);
        }

        public async Task UpdateFile(Guid fileGuid, IFormFile file)
        {
            FileData fileObject = RetrieveValidFileData(() => _fileDataRepository.GetByGuid(fileGuid));
            if (!fileObject.OriginalName.Split('.').Last().Equals(file.FileName.Split('.').Last()))
            {
                throw new Exception("Incorrect file format");
            }

            FileData? fileWithSameName = _fileDataRepository.GetByName(file.FileName);
            if (fileWithSameName != null !&& fileWithSameName.Guid.Equals(fileObject.Guid))
            {
                throw new Exception($"Can not rename file ${fileObject.OriginalName} to ${file.FileName} as it already exists!");
            }

            await _fileSystemRepository.UpdateInFileSystem(fileObject.StoredFileName(), file);
            _fileDataRepository.Update(file.ExistingFileData(fileGuid));
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

