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
        private readonly IFileStorageRepository _fileSystemRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly Session _session;

        private string AllFilesCacheKey
        {
            get
            {
                return $"{nameof(FileDetailsDTO)}_owner_{_session.UserId}";
            }
        }

        public StoredFileService(IMapper mapper, IStoredFileRepository storedFileRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileStorageRepository fileSystemRepository, ICacheRepository cacheRepository, Session session)
        {
            _mapper = mapper;
            _storedFileRepository = storedFileRepository;
            _fileSystemRepository = fileSystemRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _cacheRepository = cacheRepository;
            _session = session;
        }

        #region "Public methods"
        public async Task<List<OwnFileDetailsDTO>> GetAllFilesInfo(bool? deletedFiles)
        {
            Func<Task<List<OwnFileDetailsDTO>>> allFilesTask = async () =>
            {
                List<StoredFile> storedFiles = await _storedFileRepository.ListAsync(new OwnedFilesSpec(_session.UserId));
                return _mapper.Map<List<OwnFileDetailsDTO>>(storedFiles);
            };
            List<OwnFileDetailsDTO> fileDetailsDTOs = await _cacheRepository.GetAndSetAsync(AllFilesCacheKey, allFilesTask);

            if (deletedFiles == null)
            {
                return fileDetailsDTOs;
            }

            return fileDetailsDTOs.Where(x => x.IsDeleted == deletedFiles).ToList();
        }

        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, string? accessKey = null)
        {
            return _mapper.Map<FileDetailsDTO>(await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey));
        }

        public async Task<FileDownloadDTO> DownloadFile(Guid fileId, string? accessKey = null)
        {
            // validates the user has access to the file
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey);
            FileDownloadDTO fileDownloadDTO = _mapper.Map<FileDownloadDTO>(storedFile);
            fileDownloadDTO.Content = await _fileSystemRepository.ReadFileFromFileStorage(storedFile.FilePath());
            return fileDownloadDTO;
        }

        public async Task<FileDTO> AddFile(IFormFile file, Guid directoryId)
        {
            await _storedFileRepository.ValidateFileNameNotInDirectory(directoryId, file.FileName);
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);

            StoredFile fileToStore = new StoredFile()
            {
                OwnerId = _session.UserId,
                DirectorId = directoryId,
                Name = file.FileName,
                AccessLevel = AccessType.Private,
                SizeInBytes = file.Length,
                ContentType = file.ContentType
            };
            StoredFile storedFile = await _storedFileRepository.AddAsync(fileToStore);
            await _fileSystemRepository.AddFileInFileStorage(storedFile.FilePath(), file);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);

            return _mapper.Map<FileDTO>(storedFile);
        }

        public async Task<FileDTO> UpdateFileInfo(FileUpdateDTO fileUpdate, Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId);
            if (!string.IsNullOrEmpty(fileUpdate.Name))
            {
                await _storedFileRepository.ValidateFileNameNotInDirectory(storedFile.DirectorId, fileUpdate.Name);
                storedFile.Name = fileUpdate.Name;
            }

            if (fileUpdate.AccessLevel != null)
            {
                storedFile.AccessLevel = (AccessType)fileUpdate.AccessLevel;
            }

            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
            return _mapper.Map<FileDTO>(await _storedFileRepository.GetByIdAsync(fileId));
        }

        public async Task<FileDTO> UpdateFile(IFormFile file, Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId);
            if (file.ContentType != storedFile.ContentType)
            {
                throw new InvalidException("can not change the content type of the file");
            }

            await _fileSystemRepository.UpdateFileInFileStorage(storedFile.FilePath(), file);

            storedFile.Name = file.FileName;
            storedFile.SizeInBytes = file.Length;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
            return _mapper.Map<FileDTO>(await _storedFileRepository.GetByIdAsync(fileId));
        }

        public async Task MoveToDirectory(Guid fileId, Guid directoryId)
        {
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnActiveFileInfo(_session, fileId);
            storedFile.DirectorId = directoryId;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
        }

        public async Task MoveFileToBin(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnActiveFileInfo(_session, fileId);
            storedFile.IsDeleted = true;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
        }

        public async Task RestoreFile(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            storedFile.IsDeleted = false;
            if (storedFile.Directory.IsDeleted == true)
            {
                VirtualDirectory directory = await _virtualDirectoryRepository.ValidateAndRetrieveRootDirectoryInfo(_session.UserId);
                storedFile.DirectorId = directory.Id;
            }
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
        }

        public async Task DeleteFile(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            await _fileSystemRepository.RemoveFileFromFileStorage(storedFile.FilePath());
            await _storedFileRepository.DeleteAsync(storedFile);
        }
        #endregion
    }
}
