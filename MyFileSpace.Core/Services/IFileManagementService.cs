using Microsoft.AspNetCore.Http;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Core.Services
{
    public interface IFileManagementService
    {
        /// <summary>
        /// Retrieves the file from the local file system.
        /// </summary>
        /// <param name="fileName"> The information for the file that should be retrieved 
        /// from the local system.</param>
        /// <returns>
        /// Returns a file object, which contains all the information 
        /// about the file.
        /// </returns>
        Task<FileData> GetFileData(string fileName);


        /// <summary>
        /// Retrieves the file from the local file system.
        /// </summary>
        /// <param name="fileName"> The information for the file that should be retrieved 
        /// from the local system.</param>
        /// <returns>
        /// Returns a file object, which contains all the information 
        /// about the file.
        /// </returns>
        Task<byte[]> GetFileByGuid(Guid fileGuid);

        /// <summary>
        /// Saves a file in the local file system.
        /// </summary>
        /// <param name="file"> All the information for the file that must be saved 
        /// in the local system.</param>
        Task AddFileAsync(IFormFile file);

        /// <summary>
        /// Updates a file in the local file system.
        /// </summary>
        /// <param name="file"> All the information for the file that must be saved 
        /// in the local system.</param>
        Task UpdateFileAsync(Guid fileGuid, IFormFile file);

        /// <summary>
        /// Deletes a file from the local file system.
        /// </summary>
        /// <param name="fileGuid"> The name of the file that should be removed from the 
        /// localfile system.</param>
        /// <returns>
        /// Returns <code>true</code> in case the removal of the file was successfull, or 
        /// <code>false</code> otherwise.
        /// </returns>
        bool DeleteFile(Guid fileGuid);
    }
}
