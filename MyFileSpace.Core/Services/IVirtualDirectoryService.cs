using Microsoft.AspNetCore.Http;
using MyFileSpace.Core.DTOs;

namespace MyFileSpace.Core.Services
{
    public interface IVirtualDirectoryService
    {
        /// <summary>
        /// Retrieves all directories information for the current user.
        /// </summary>
        /// <returns>
        /// Returns a list of directory details.
        /// </returns>
        Task<List<DirectoryDTO>> GetAllDirectoriesInfo();

        /// <summary>
        /// Retrieves the directory information.
        /// </summary>
        /// <param name="directoryId"> The information for the directory that should be retrieved 
        /// from the local system.</param>
        /// <param name="accessKey"> Access key in case of anonymous user
        /// <returns>
        /// Returns a directory object, which contains all the information 
        /// about the directory.
        /// </returns>
        Task<DirectoryDetailsDTO> GetDirectoryInfo(Guid directoryId, string? accessKey = null);

        /// <summary>
        /// Adds a virtual directory for the current user.
        /// </summary>
        /// <param name="directory"> All the information for the directory that must be saved.</param>
        /// <param name="parentDirectoryId"> The directory where the directory should be saved </param>
        Task<DirectoryDTO> AddDirectory(DirectoryUpdateDTO directory, Guid parentDirectoryId);

        /// <summary>
        /// Updates a virtual directory.
        /// </summary>
        /// <param name="directoryUpdate"> All the information for the directory.</param>
        Task<DirectoryDTO> UpdateDirectory(DirectoryUpdateDTO directoryUpdate, Guid directoryId);

        /// <summary>
        /// Moves a directory to another directory.
        /// </summary>
        /// <param name="directoryToMoveId"> The directory that needs to be moved.</param>
        /// <param name="newParentDirectoryId"> The new parent directory id for the directory that should be moved.</param>
        Task MoveDirectory(Guid directoryToMoveId, Guid newParentDirectoryId, bool restore);

        /// <summary>
        /// Deletes a virtual directory and all its content.
        /// </summary>
        /// <param name="directoryId"> The name of the directory that should be removed.</param>
        Task DeleteDirectory(Guid directoryId, bool permanent);
    }
}
