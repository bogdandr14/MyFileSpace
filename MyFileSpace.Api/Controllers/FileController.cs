using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Api.Filters;
using MyFileSpace.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomExceptionFilter]

    public class FileController : ControllerBase
    {
        private readonly IFileManagementService _fileManagementService;

        public FileController(IFileManagementService fileManagementService)
        {
            _fileManagementService = fileManagementService;
        }

        // GET: api/<FileController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await _fileManagementService.GetAllFileNames();
        }

        // GET api/<FileController>/5
        [HttpGet("info/{fileName}")]
        public async Task<SharedKernel.DTOs.FileDTO_old> GetInfo(string fileName)
        {
            return await _fileManagementService.GetFileData(fileName);
        }

        // GET api/<FileController>/download/5
        [HttpGet("download/{fileName}")]
        public async Task<ActionResult> DownloadFile(string fileName)
        {
            byte[] fileContent = await _fileManagementService.GetFileByName(fileName);
            FileContentResult fileContentResult = File(fileContent, "application/octet-stream");
            fileContentResult.FileDownloadName = fileName;

            SharedKernel.DTOs.FileDTO_old fileData = await _fileManagementService.GetFileData(fileName);
            fileContentResult.LastModified = fileData.ModifiedAt;
            return fileContentResult;
        }

        // POST api/<FileController>
        [HttpPost]
        public async Task Post([FileValidation(4096)] IFormFile uploadedFile)
        {
            await _fileManagementService.AddFile(uploadedFile);
        }

        // PUT api/<FileController>/5
        [HttpPut("{guid:Guid}")]
        public async Task Put(Guid guid, [FileValidation(4096)] IFormFile uploadedFile)
        {
            await _fileManagementService.UpdateFile(guid, uploadedFile);
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{guid:Guid}")]
        public async Task Delete(Guid guid)
        {
            await _fileManagementService.DeleteFile(guid);
        }
    }
}
