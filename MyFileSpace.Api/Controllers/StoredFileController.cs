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
        public async Task<ActionResult<List<FileDetailsDTO>>> GetDirectories()
        {
            try
            {
                return Ok(await _storedFileService.GetAllFilesInfo());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("info/{fileId: Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult<FileDetailsDTO>> GetFileInfo(Guid fileId)
        {
            try
            {
                return Ok(await _storedFileService.GetFileInfo(fileId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("info/{fileId: Guid}/{accessKey:string}")]
        public async Task<ActionResult<FileDetailsDTO>> GetFileInfo(Guid fileId, string accessKey)
        {
            try
            {
                return Ok(await _storedFileService.GetFileInfo(fileId, accessKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [MyFileSpaceAuthorize]
        [HttpPost("{directoryId:Guid}")]
        public async Task<ActionResult> AddFile([FileValidation(4096)] IFormFile uploadedFile, Guid directoryId)
        {
            try
            {
                await _storedFileService.AddFile(uploadedFile, directoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> UpdateFile([FileValidation(4096)] IFormFile uploadedFile, Guid fileId)
        {
            try
            {
                await _storedFileService.UpdateFile(uploadedFile, fileId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("info/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> UpdateFileInfo(FileUpdateDTO fileUpdateDTO, Guid fileId)
        {
            try
            {
                await _storedFileService.UpdateFileInfo(fileUpdateDTO, fileId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("move/{fileId:Guid}/{directoryId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> MoveToDirectory(Guid fileId, Guid directoryId)
        {
            try
            {
                await _storedFileService.MoveToDirectory(fileId, directoryId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("restore/{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> RestoreFile(Guid fileId)
        {
            try
            {
                await _storedFileService.RestoreFile(fileId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{fileId:Guid}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> MoveToBin(Guid fileId)
        {
            try
            {
                await _storedFileService.MoveFileToBin(fileId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("permanent/{fileId:Guid}")]
        [MyFileSpaceAuthorize]

        public async Task<ActionResult> Delete(Guid fileId)
        {
            try
            {
                await _storedFileService.DeleteFile(fileId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
