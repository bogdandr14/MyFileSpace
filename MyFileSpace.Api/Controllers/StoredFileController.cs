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

        [HttpPost("upload/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UploadNewFile([FileValidation(400)] IFormFile uploadedFile, Guid directoryId)
        {
            return await _storedFileService.UploadNewFile(uploadedFile, directoryId);
        }

        [HttpPut("upload/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UploadExistingFile([FileValidation(400)] IFormFile uploadedFile, Guid fileId)
        {
            return await _storedFileService.UploadExistingFile(uploadedFile, fileId);
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
            return File(fileContent.ContentStream, "application/octet-stream", fileContent.DownloadName, fileContent.LastModified, null!);
        }

        [HttpPut("move/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveFile(Guid fileId, [FromQuery] Guid directoryId, [FromQuery] bool restore = false)
        {
            await _storedFileService.MoveFile(fileId, directoryId, restore);
        }

        [HttpDelete("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task Delete(Guid fileId, [FromQuery] bool permanent = false)
        {
            await _storedFileService.DeleteFile(fileId, permanent);
        }
    }
}
