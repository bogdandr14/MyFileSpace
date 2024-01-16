using Microsoft.AspNetCore.Mvc;
using MyFileStorage.Api.Attributes;
using MyFileStorage.Api.DTO;
using MyFileStorage.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFileStorage.Api.Controllers
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
        [HttpGet("{id}")]
        public FileDTO Get(int id)
        {
            return _fileManagementService.GetFile(id.ToString()).Result;
        }

        // POST api/<FileController>
        [HttpPost]
        public void Post([FromForm][FileValidation(4096)] IFormFile uploadedFile)
        {
            _fileManagementService.AddFileAsync(uploadedFile);
        }

        // PUT api/<FileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromForm][FileValidation(4096)] IFormFile uploadedFile)
        {
            _fileManagementService.UpdateFileAsync(id, uploadedFile);
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _fileManagementService.DeleteFile(id.ToString());
        }
    }
}
