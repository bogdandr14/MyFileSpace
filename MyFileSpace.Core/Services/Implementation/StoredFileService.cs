using AutoMapper;
using Microsoft.AspNetCore.Http;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class StoredFileService : IStoredFileService
    {
        private readonly IMapper _mapper;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly Session _session;

        private string AllFilesCacheKey
        {
            get
            {
                return $"{nameof(FileDetailsDTO)}_owner_{_session.UserId}";
            }
        }

        public StoredFileService(IMapper mapper, IStoredFileRepository storedFileRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileSystemRepository fileSystemRepository, ICacheRepository cacheRepository, Session session)
        {
            _mapper = mapper;
            _storedFileRepository = storedFileRepository;
            _fileSystemRepository = fileSystemRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _cacheRepository = cacheRepository;
            _session = session;
        }

        #region "Public methods"
        public async Task<List<FileDetailsDTO>> GetAllFilesInfo()
        {
            Func<Task<List<FileDetailsDTO>>> allFilesTask = async () =>
            {
                List<StoredFile> storedFiles = await _storedFileRepository.ListAsync(new OwnedFilesSpec(_session.UserId));
                return _mapper.Map<List<FileDetailsDTO>>(storedFiles);
            };
            return await _cacheRepository.GetAndSetAsync(AllFilesCacheKey, allFilesTask);
        }

        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, string? accessKey = null)
        {
            return _mapper.Map<FileDetailsDTO>(await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey));
        }

        public async Task<byte[]> DownloadFile(Guid fileId, string? accessKey = null)
        {
            // validates the user has access to the file
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey);

            return await _fileSystemRepository.ReadFromFileSystem(StoredFilePath(storedFile));
        }

        public async Task AddFile(IFormFile file, Guid directoryId)
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
                State = true
            };
            StoredFile storedFile = await _storedFileRepository.AddAsync(fileToStore);
            await _fileSystemRepository.AddInFileSystem(StoredFilePath(storedFile), file);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
        }

        public async Task UpdateFileInfo(FileUpdateDTO fileUpdate, Guid fileId)
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
        }

        public async Task UpdateFile(IFormFile file, Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId);
            string[] storedFileNameSplit = storedFile.Name.Split('.');
            if (storedFileNameSplit.Length > 1)
            {
                //TODO handle different content type
            }
            await _fileSystemRepository.UpdateInFileSystem(StoredFilePath(storedFile), file);

            storedFile.Name = file.FileName;
            storedFile.SizeInBytes = file.Length;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
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
            storedFile.State = false;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
        }

        public async Task RestoreFile(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            storedFile.State = true;
            if (storedFile.Directory.State == false)
            {
                VirtualDirectory directory = storedFile.Directory;
                while (directory.ParentDirectory != null)
                {
                    directory = directory.ParentDirectory;
                }
                storedFile.DirectorId = directory.Id;
            }
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheRepository.RemoveAsync(AllFilesCacheKey);
        }

        public async Task DeleteFile(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            await _fileSystemRepository.RemoveFromFileSystem(StoredFilePath(storedFile));
            await _storedFileRepository.DeleteAsync(storedFile);
        }
        #endregion

        private string StoredFilePath(StoredFile storedFile)
        {
            return $"{storedFile.OwnerId}/{storedFile.Id}";
        }
    }
}
