using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;
using MyFileSpace.Infrastructure.Persistence.Entities;

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
        public async Task<List<OwnFileDetailsDTO>> GetFiles([FromQuery] bool? deletedFiles)
        {
            return await _storedFileService.GetAllFilesInfo(deletedFiles);
        }

        [HttpGet("{fileId:Guid}")]
        [MyFileSpaceAuthorize(true)]
        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, [FromQuery] string? accessKey)
        {
            return await _storedFileService.GetFileInfo(fileId, accessKey);
        }

        [MyFileSpaceAuthorize]
        [HttpPost("upload/{directoryId:Guid}")]
        public async Task<FileDTO> AddFile([FileValidation(4096)] IFormFile uploadedFile, Guid directoryId)
        {
            return await _storedFileService.AddFile(uploadedFile, directoryId);
        }

        [HttpPut("upload/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UpdateFile([FileValidation(4096)] IFormFile uploadedFile, Guid fileId)
        {
            return await _storedFileService.UpdateFile(uploadedFile, fileId);
        }

        [HttpPut("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UpdateFileInfo(FileUpdateDTO fileUpdateDTO, Guid fileId)
        {
            return await _storedFileService.UpdateFileInfo(fileUpdateDTO, fileId);
        }

        [HttpGet("download/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> DownloadFile(Guid fileId, [FromQuery] string? accessKey = null)
        {
            FileDownloadDTO fileContent = await _storedFileService.DownloadFile(fileId, accessKey);
            return File(fileContent.Content, "application/octet-stream", fileContent.DownloadName, fileContent.LastModified, null!);
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
