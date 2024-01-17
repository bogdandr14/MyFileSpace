using AutoMapper;
using Microsoft.AspNetCore.Http;
using MyFileSpace.Api.Extensions;
using MyFileSpace.Core.Providers;
using MyFileSpace.Core.Repositories;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Core.Services
{
    public class FileManagementService : IFileManagementService
    {
        private const string _filesDirectoryConfiguration = "FilesDirectory";
        private readonly IMapper _mapper;

        private readonly string _fileDirectoryPath;

        public FileManagementService(IMapper mapper, IAppConfigurationProvider configurationProvider)
        {
            _mapper = mapper;
            string relativeFileDirectoryPath = configurationProvider.GetValue(_filesDirectoryConfiguration);
            _fileDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), relativeFileDirectoryPath);
            Directory.CreateDirectory(_fileDirectoryPath);
        }

        public async Task<FileData> GetFileData(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception("No filename given");
            }

            FileData fileObject = new CsvFileRepository().GetByName(fileName);
            return fileObject;
        }
        public async Task<byte[]> GetFileByGuid(Guid fileGuid)
        {
            FileData? fileObject = new CsvFileRepository().GetByGuid(fileGuid);
            return await File.ReadAllBytesAsync(Path.Combine(_fileDirectoryPath, fileObject.Path));
        }

        public async Task AddFileAsync(IFormFile file)
        {
            FileData fileData = file.ToFileData();
            string fullPath = Path.Combine(_fileDirectoryPath, fileData.Path);

            using (FileStream fileStream = File.Create(fullPath))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                await file.OpenReadStream().CopyToAsync(fileStream);
            }

            new CsvFileRepository().Add(file.ToFileData());
        }

        public async Task UpdateFileAsync(Guid fileGuid, IFormFile file)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFile(Guid fileGuid)
        {
            string path = Path.Combine(_fileDirectoryPath, new CsvFileRepository().GetByGuid(fileGuid).Path);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}

