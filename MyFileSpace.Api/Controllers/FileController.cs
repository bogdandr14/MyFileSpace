using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.Services;
using MyFileSpace.SharedKernel.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileManagementService _fileManagementService;

        public FileController(IFileManagementService fileManagementService)
        {
            _fileManagementService = fileManagementService;
        }

        // GET: api/<FileController>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try
            {
                return Ok(_fileManagementService.GetAllFileNames());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // GET api/<FileController>/5
        [HttpGet("info/{fileName}")]
        public ActionResult<FileDTO> GetInfo(string fileName)
        {
            try
            {
                return Ok(_fileManagementService.GetFileData(fileName));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // GET api/<FileController>/download/5
        [HttpGet("download/{fileName}")]
        public async Task<ActionResult> DownloadFile(string fileName)
        {
            try
            {
                byte[] fileContent = await _fileManagementService.GetFileByName(fileName);
                FileContentResult fileContentResult = File(fileContent, "application/octet-stream");
                fileContentResult.FileDownloadName = fileName;

                FileDTO fileData = _fileManagementService.GetFileData(fileName);
                fileContentResult.LastModified = fileData.ModifiedAt;
                return fileContentResult;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST api/<FileController>
        [HttpPost]
        public async Task<ActionResult> Post([FileValidation(4096)] IFormFile uploadedFile)
        {
            try
            {
                await _fileManagementService.AddFile(uploadedFile);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // PUT api/<FileController>/5
        [HttpPut("{guid:Guid}")]
        public async Task<ActionResult> Put(Guid guid, [FileValidation(4096)] IFormFile uploadedFile)
        {
            try
            {
                await _fileManagementService.UpdateFile(guid, uploadedFile);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{guid:Guid}")]
        public async Task<ActionResult> Delete(Guid guid)
        {
            try
            {
                await _fileManagementService.DeleteFile(guid);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
