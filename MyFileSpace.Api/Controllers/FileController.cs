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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FileController>/5
        [HttpGet("info/{fileName}")]
        public FileData GetInfo(string fileName)
        {
            return _fileManagementService.GetFileData(fileName).Result;
        }

        // GET api/<FileController>/5
        [HttpGet("{guid: Guid}")]
        public byte[] Get(Guid guid)
        {
            return _fileManagementService.GetFileByGuid(guid).Result;
        }

        // POST api/<FileController>
        [HttpPost]
        public void Post([FromForm][FileValidation(4096)] IFormFile uploadedFile)
        {
            _fileManagementService.AddFileAsync(uploadedFile);
        }

        // PUT api/<FileController>/5
        [HttpPut("{id:Guid}")]
        public void Put(Guid guid, [FromForm][FileValidation(4096)] IFormFile uploadedFile)
        {
            _fileManagementService.UpdateFileAsync(guid, uploadedFile);
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{guid:Guid}")]
        public void Delete(Guid guid)
        {
            _fileManagementService.DeleteFile(guid);
        }
    }
}
