using AutoMapper;
using Microsoft.AspNetCore.Http;
using MyFileSpace.Core.Providers;
using MyFileSpace.SharedKernel.DTO;

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

        public async Task<FileDTO> GetFile(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new Exception("No filename given");
            }

            FileDTO fileObject = _mapper.Map<FileDTO>(fileName);
            fileObject.FileInBytes = await File.ReadAllBytesAsync(Path.Combine(_fileDirectoryPath, fileName));

            return fileObject;
        }

        public async Task AddFileAsync(IFormFile file)
        {
            string fullPath = Path.Combine(_fileDirectoryPath, file.FileName);

            using (FileStream fileStream = File.Create(fullPath))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                await file.OpenReadStream().CopyToAsync(fileStream);
            }
        }
        public async Task UpdateFileAsync(int id, IFormFile file)
        {
            throw new NotImplementedException();
        }
        public bool DeleteFile(string fileName)
        {
            string path = Path.Combine(_fileDirectoryPath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}

