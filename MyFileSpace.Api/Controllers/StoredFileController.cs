using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.Enums;

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
        public async Task<List<FileDTO>> GetFiles([FromQuery] bool? deleted)
        {
            return await _storedFileService.GetAllFilesInfo(deleted ?? false);
        }

        [HttpGet("search")]
        [MyFileSpaceAuthorize(true)]
        public async Task<FilesFoundDTO> SearchFiles([FromQuery] InfiniteScrollFilter filter)
        {
            return await _storedFileService.SearchFiles(filter);
        }

        [HttpGet("{fileId:Guid}")]
        [MyFileSpaceAuthorize(true)]
        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, [FromQuery] string? accessKey)
        {
            return await _storedFileService.GetFileInfo(fileId, accessKey);
        }

        [HttpPost("upload/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UploadNewFile([FileValidation(400)] IFormFile file, Guid directoryId,[FromForm] AccessType accessLevel)
        {
            return await _storedFileService.UploadNewFile(file, directoryId, accessLevel);
        }

        [HttpPut("upload/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UploadExistingFile([FileValidation(400)] IFormFile file, Guid fileId)
        {
            return await _storedFileService.UploadExistingFile(file, fileId);
        }

        [HttpPut]
        [MyFileSpaceAuthorize]
        public async Task<FileDTO> UpdateFileInfo(FileUpdateDTO fileUpdateDTO)
        {
            return await _storedFileService.UpdateFileInfo(fileUpdateDTO);
        }

        [HttpGet("download/{fileId:Guid}")]
        [MyFileSpaceAuthorize(true)]
        public async Task<ActionResult> DownloadFile(Guid fileId, [FromQuery] string? accessKey = null)
        {
            FileDownloadDTO fileContent = await _storedFileService.DownloadFile(fileId, accessKey);
            return File(fileContent.ContentStream, "application/octet-stream", fileContent.DownloadName);
        }

        [HttpPut("move/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task MoveFile(Guid fileId, [FromQuery] Guid directoryId, [FromQuery] bool restore = false)
        {
            await _storedFileService.MoveFile(fileId, directoryId, restore);
        }

        [HttpPost("favorite/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task AddFavorite(Guid fileId)
        {
            await _storedFileService.AddToFavorites(fileId);
        }

        [HttpDelete("favorite/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task RemoveFavorite(Guid fileId)
        {
            await _storedFileService.RemoveFromFavorites(fileId);
        }

        [HttpDelete("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task Delete(Guid fileId, [FromQuery] bool permanent = false)
        {
            await _storedFileService.DeleteFile(fileId, permanent);
        }
    }
}
