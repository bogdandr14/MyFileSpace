﻿using Microsoft.AspNetCore.Http;
using MyFileSpace.Caching;
using MyFileSpace.Core.DTOs;
using MyFileSpace.SharedKernel;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services
{
    public interface IStoredFileService
    {

        Task<FileStatisticsDTO> GetStatistics();

        Task<MemorySize> GetAllowedStorage();
        /// <summary>
        /// </summary>
        /// <returns>
        /// Returns a list of file details.
        /// </returns>
        Task<FilesFoundDTO> SearchFiles(InfiniteScrollFilter filter);

        /// <summary>
        /// Retrieves all files information for the stored files in the local file system.
        /// </summary>
        /// <returns>
        /// Returns a list of file details.
        /// </returns>
        Task<List<FileDTO>> GetAllFilesInfo(bool deletedFiles);

        /// <summary>
        /// Retrieves the file information.
        /// </summary>
        /// <param name="fileId"> The information for the file that should be retrieved 
        /// from the local system.</param>
        /// <param name="accessKey"> Access key in case of anonymous user
        /// <returns>
        /// Returns a file object, which contains all the information 
        /// about the file.
        /// </returns>
        Task<FileDetailsDTO> GetFileInfo(Guid fileId, string? accessKey = null);

        /// <summary>
        /// Retrieves the file from the local file system.
        /// </summary>
        /// <param name="fileId"> The file that should be retrieved from the local system.</param>
        /// <returns>
        /// Downloads a file.
        /// </returns>
        Task<FileDownloadDTO> DownloadFile(Guid fileId, string? accessKey = null);

        /// <summary>
        /// Saves a file in the local file system.
        /// </summary>
        /// <param name="file"> All the information for the file that must be saved in the local system.</param>
        /// <param name="directoryId"> The directory where the file should be saved </param>
        Task<FileDTO> UploadNewFile(IFormFile file, Guid directoryId, AccessType accessLevel);

        /// <summary>
        /// Updates a file in the local file system.
        /// </summary>
        /// <param name="file"> All the information for the file that must be saved in the local system.</param>
        Task<FileDTO> UploadExistingFile(IFormFile file, Guid fileId);

        /// <summary>
        /// Updates a file innformation.
        /// </summary>
        /// <param name="fileUpdate"> All the information for the file that must be saved in the local system.</param>
        /// <param name="fileUpdate"> The id of the file that needs to be updated.</param>
        Task<FileDTO> UpdateFileInfo(FileUpdateDTO fileUpdate);

        /// <summary>
        /// Move a file to a specific directory, keeping it in the local file system.
        /// </summary>
        /// <param name="fileId"> The file id that should be moved in the localfile system.</param>
        /// <param name="directoryId"> The directory id where the file should be moved.</param>
        Task MoveFile(Guid fileId, Guid directoryId, bool restore);

        Task AddToFavorites(Guid fileId);

        Task RemoveFromFavorites(Guid fileId);

        /// <summary>
        /// Deletes a file from the local file system.
        /// </summary>
        /// <param name="fileId"> The name of the file that should be removed from the localfile system.</param>
        Task DeleteFile(Guid fileId, bool permanent);

        Task DeletePastBinRetention();
    }
}
