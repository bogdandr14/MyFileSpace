using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoredFileController : ControllerBase
    {
        private readonly IStoredFileService _storedFileService;

        public StoredFileController(IStoredFileService storedFileService)
        {
            _storedFileService = storedFileService;
        }

        [HttpGet]
        [MyFileSpaceAuthorize]
        public async Task<List<FileDetailsDTO>> GetDirectories()
        {
            return await _storedFileService.GetAllFilesInfo();
        }

        [HttpGet("info/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId)
        {
            return await _storedFileService.GetFileInfo(fileId);
        }

        [HttpGet("info/{fileId:Guid}/{accessKey}")]
        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, string accessKey)
        {
            return await _storedFileService.GetFileInfo(fileId, accessKey);
        }

        [MyFileSpaceAuthorize]
        [HttpPost("{directoryId:Guid}")]
        public async Task AddFile([FileValidation(4096)] IFormFile uploadedFile, Guid directoryId)
        {
            await _storedFileService.AddFile(uploadedFile, directoryId);
        }

        [HttpPut("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task UpdateFile([FileValidation(4096)] IFormFile uploadedFile, Guid fileId)
        {
            await _storedFileService.UpdateFile(uploadedFile, fileId);
        }

        [HttpPut("info/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task UpdateFileInfo(FileUpdateDTO fileUpdateDTO, Guid fileId)
        {
            await _storedFileService.UpdateFileInfo(fileUpdateDTO, fileId);
        }

        [HttpPut("move/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveToDirectory(Guid fileId, Guid directoryId)
        {
            await _storedFileService.MoveToDirectory(fileId, directoryId);
        }

        [HttpPut("restore/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task RestoreFile(Guid fileId)
        {
            await _storedFileService.RestoreFile(fileId);
        }

        [HttpDelete("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveToBin(Guid fileId)
        {
            await _storedFileService.MoveFileToBin(fileId);
        }

        [HttpDelete("permanent/{fileId:Guid}")]
        [MyFileSpaceAuthorize]

        public async Task Delete(Guid fileId)
        {
            await _storedFileService.DeleteFile(fileId);
        }
    }
}
