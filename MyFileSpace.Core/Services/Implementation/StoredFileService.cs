using AutoMapper;
using Microsoft.AspNetCore.Http;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class StoredFileService : IStoredFileService
    {
        private readonly IMapper _mapper;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IFileStorageRepository _fileStorageRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly Session _session;

        public StoredFileService(IMapper mapper, IStoredFileRepository storedFileRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileStorageRepository fileSystemRepository, ICacheRepository cacheRepository, Session session)
        {
            _mapper = mapper;
            _storedFileRepository = storedFileRepository;
            _fileStorageRepository = fileSystemRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _cacheRepository = cacheRepository;
            _session = session;
        }

        #region "Public methods"

        public async Task<FilesFoundDTO> SearchFiles(InfiniteScrollFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Name))
            {
                filter.Name = " ";
            }
            //increase to check if last files
            filter.Take++;
            List<StoredFile> storedFiles;
            if (_session.IsAuthenticated)
            {
                storedFiles = await _storedFileRepository.ListAsync(new SearchFilesSpec(filter, _session.UserId));
            }
            else
            {
                storedFiles = await _storedFileRepository.ListAsync(new SearchFilesSpec(filter));
            }

            FilesFoundDTO filesFound = new FilesFoundDTO();
            filesFound.Skipped = filter.Skip;
            filesFound.AreLast = storedFiles.Count < filter.Take;
            if (!filesFound.AreLast)
            {
                storedFiles.RemoveAt(storedFiles.Count - 1);
            }
            filesFound.Taken = storedFiles.Count;
            filesFound.Items = _mapper.Map<List<FileDTO>>(storedFiles);

            return filesFound;
        }
        public async Task<List<OwnFileDetailsDTO>> GetAllFilesInfo(bool? deletedFiles)
        {
            Func<Task<List<OwnFileDetailsDTO>>> allFilesTask = async () =>
            {
                List<StoredFile> storedFiles = await _storedFileRepository.ListAsync(new OwnedFilesWithDirectoriesSpec(_session.UserId));
                return _mapper.Map<List<OwnFileDetailsDTO>>(storedFiles);
            };
            List<OwnFileDetailsDTO> fileDetailsDTOs = await _cacheRepository.GetAndSetAsync(_session.AllFilesCacheKey, allFilesTask);

            if (deletedFiles == null)
            {
                return fileDetailsDTOs;
            }

            return fileDetailsDTOs.Where(x => x.IsDeleted == deletedFiles).ToList();
        }

        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, string? accessKey = null)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey);
            FileDetailsDTO fileDetailsDTO = _mapper.Map<FileDetailsDTO>(storedFile);
            if (fileDetailsDTO.OwnerId.Equals(_session.UserId))
            {
                fileDetailsDTO.AllowedUsers = storedFile.AllowedUsers.Select(x => x.AllowedUser.TagName).ToList();
                if (storedFile.FileAccessKey != null)
                {
                    fileDetailsDTO.AccessKey = _mapper.Map<KeyAccessDetailsDTO>(storedFile.FileAccessKey.AccessKey);
                    if (fileDetailsDTO.AccessKey.ExpiresAt == DateTime.MaxValue)
                    {
                        fileDetailsDTO.AccessKey.ExpiresAt = null;
                    }
                }
            }
                
            return fileDetailsDTO;
        }

        public async Task<FileDownloadDTO> DownloadFile(Guid fileId, string? accessKey = null)
        {
            // validates the user has access to the file
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey);
            FileDownloadDTO fileDownloadDTO = _mapper.Map<FileDownloadDTO>(storedFile);
            fileDownloadDTO.ContentStream = await _fileStorageRepository.ReadFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString());
            return fileDownloadDTO;
        }

        public async Task<FileDTO> UploadNewFile(IFormFile file, Guid directoryId, AccessType accessLevel)
        {
            await _storedFileRepository.ValidateFileNameNotInDirectory(directoryId, file.FileName);
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);
            await _storedFileRepository.ValidateOwnFileEnoughSpace(_session.UserId, file.Length);

            StoredFile fileToStore = new StoredFile()
            {
                OwnerId = _session.UserId,
                DirectorId = directoryId,
                Name = file.FileName,
                AccessLevel = accessLevel,
                SizeInBytes = file.Length,
                ContentType = file.ContentType
            };
            StoredFile storedFile = await _storedFileRepository.AddAsync(fileToStore);
            Task.WaitAll(
                _fileStorageRepository.UploadFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString(), file),
                _cacheRepository.RemoveAsync(_session.AllFilesCacheKey)
            );

            return _mapper.Map<FileDTO>(storedFile);
        }

        public async Task<FileDTO> UpdateFileInfo(FileUpdateDTO fileUpdate)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileUpdate.FileId);
            if (!string.IsNullOrEmpty(fileUpdate.Name) && storedFile.Name != fileUpdate.Name)
            {
                await _storedFileRepository.ValidateFileNameNotInDirectory(storedFile.DirectorId, fileUpdate.Name);
                storedFile.Name = fileUpdate.Name;
            }

            if (fileUpdate.AccessLevel != null)
            {
                storedFile.AccessLevel = (AccessType)fileUpdate.AccessLevel;
            }

            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(_session.AllFilesCacheKey);
            return _mapper.Map<FileDTO>(await _storedFileRepository.GetByIdAsync(fileUpdate.FileId));
        }

        public async Task<FileDTO> UploadExistingFile(IFormFile file, Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId);
            if (file.ContentType != storedFile.ContentType)
            {
                throw new InvalidException("can not change the content type of the file");
            }
            await _storedFileRepository.ValidateOwnFileEnoughSpace(_session.UserId, file.Length - storedFile.SizeInBytes);

            await _fileStorageRepository.UploadFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString(), file);

            storedFile.Name = file.FileName;
            storedFile.SizeInBytes = file.Length;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(_session.AllFilesCacheKey);
            return _mapper.Map<FileDTO>(await _storedFileRepository.GetByIdAsync(fileId));
        }

        public async Task MoveFile(Guid fileId, Guid directoryId, bool restore)
        {
            if (restore)
            {
                await RestoreFile(fileId, directoryId);
            }
            else
            {
                await MoveToDirectory(fileId, directoryId);
            }
            await _cacheRepository.RemoveAsync(_session.AllFilesCacheKey);
        }

        public async Task DeleteFile(Guid fileId, bool permanent)
        {
            if (permanent)
            {
                await DeleteFilePermanently(fileId);
            }
            else
            {
                await MoveFileToBin(fileId);
            }
            await _cacheRepository.RemoveAsync(_session.AllFilesCacheKey);
        }
        #endregion

        #region "Private methods"
        private async Task RestoreFile(Guid fileId, Guid directoryId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            storedFile.IsDeleted = false;
            if (!Guid.Empty.Equals(directoryId))
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);
                storedFile.DirectorId = directoryId;
            }
            else if (storedFile.Directory.IsDeleted == true)
            {
                VirtualDirectory directory = await _virtualDirectoryRepository.ValidateAndRetrieveRootDirectoryInfo(_session.UserId);
                storedFile.DirectorId = directory.Id;
            }
            await _storedFileRepository.UpdateAsync(storedFile);
        }

        private async Task MoveToDirectory(Guid fileId, Guid directoryId)
        {
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnActiveFileInfo(_session, fileId);
            storedFile.DirectorId = directoryId;
            await _storedFileRepository.UpdateAsync(storedFile);
        }

        private async Task MoveFileToBin(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnActiveFileInfo(_session, fileId);
            storedFile.IsDeleted = true;
            await _storedFileRepository.UpdateAsync(storedFile);
        }

        private async Task DeleteFilePermanently(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            await _fileStorageRepository.RemoveFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString());
            await _storedFileRepository.DeleteAsync(storedFile);
        }
        #endregion
    }
}
