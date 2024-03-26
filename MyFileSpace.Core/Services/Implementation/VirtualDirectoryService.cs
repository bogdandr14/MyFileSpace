using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class VirtualDirectoryService : IVirtualDirectoryService
    {
        private readonly IMapper _mapper;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IFileSystemRepository _fileSystemRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly Session _session;

        private string AllDirectoriesCacheKey
        {
            get
            {
                return $"{nameof(DirectoryDTO)}_owner_{_session.UserId}";
            }
        }

        public VirtualDirectoryService(IMapper mapper, IStoredFileRepository storedFileRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileSystemRepository fileSystemRepository, ICacheRepository cacheRepository, Session session)
        {
            _mapper = mapper;
            _storedFileRepository = storedFileRepository;
            _fileSystemRepository = fileSystemRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _cacheRepository = cacheRepository;
            _session = session;
        }

        public async Task<List<DirectoryDTO>> GetAllDirectoriesInfo()
        {
            Func<Task<List<DirectoryDTO>>> allDirectoriesTask = async () =>
            {
                List<VirtualDirectory> virtualDirectories = await _virtualDirectoryRepository.ListAsync(new OwnedDirectoriesSpec(_session.UserId));
                return _mapper.Map<List<DirectoryDTO>>(virtualDirectories);
            };
            return await _cacheRepository.GetAndSetAsync(AllDirectoriesCacheKey, allDirectoriesTask);
        }

        public async Task<DirectoryDetailsDTO> GetDirectoryInfo(Guid directoryId, string? accessKey = null)
        {
            Func<Task<DirectoryDetailsDTO>> directoryDetailsTask = async () =>
            {
                VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveDirectoryInfo(_session, directoryId, accessKey);
                DirectoryDetailsDTO directoryDTO = _mapper.Map<DirectoryDetailsDTO>(virtualDirectory);
                if (virtualDirectory.Owner.Id.Equals(_session.UserId))
                {
                    directoryDTO.Files = _mapper.Map<List<FileDTO>>(virtualDirectory.FilesInDirectory);
                    directoryDTO.ChildDirectories = _mapper.Map<List<DirectoryDTO>>(virtualDirectory.ChildDirectories);
                    directoryDTO.AllowedUsers = virtualDirectory.AllowedUsers.Select(x => x.AllowedUser.TagName).ToList();
                    if (virtualDirectory.DirectoryAccessKey != null)
                    {
                        directoryDTO.AccessKey = virtualDirectory.DirectoryAccessKey.AccessKey.Key;
                    }
                }
                else
                {
                    directoryDTO.Files = _mapper.Map<List<FileDTO>>(virtualDirectory.FilesInDirectory.Where(x => x.AccessLevel == AccessType.Public || (x.AccessLevel == AccessType.Restricted && x.AllowedUsers.Any(w => w.AllowedUserId.Equals(_session.UserId)))));
                    directoryDTO.ChildDirectories = _mapper.Map<List<DirectoryDTO>>(virtualDirectory.ChildDirectories.Where(x => x.AccessLevel == AccessType.Public || (x.AccessLevel == AccessType.Restricted && x.AllowedUsers.Any(w => w.AllowedUserId.Equals(_session.UserId)))));
                }

                return directoryDTO;
            };
            return await _cacheRepository.GetAndSetAsync(DirectoryDetailsCacheKey(directoryId, accessKey), directoryDetailsTask);

        }

        public async Task AddDirectory(DirectoryUpdateDTO directory, Guid parentDirectoryId)
        {
            await _virtualDirectoryRepository.ValidateDirectoryNotInParentDirectory(parentDirectoryId, directory.Path);
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, parentDirectoryId);
            VirtualDirectory virtualDirectory = _mapper.Map<VirtualDirectory>(directory);
            virtualDirectory.OwnerId = _session.UserId;
            virtualDirectory.ParentDirectoryId = parentDirectoryId;
            await _virtualDirectoryRepository.AddAsync(virtualDirectory);
            await _cacheRepository.RemoveAsync(AllDirectoriesCacheKey);
        }

        public async Task UpdateDirectory(DirectoryUpdateDTO directoryUpdate, Guid directoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveDirectoryInfo(_session, directoryId);

            if (!string.IsNullOrEmpty(directoryUpdate.Path))
            {
                if (virtualDirectory.ParentDirectoryId == null)
                {
                    throw new Exception("can not change root path");
                }

                await _virtualDirectoryRepository.ValidateDirectoryNotInParentDirectory(virtualDirectory.ParentDirectoryId.Value, directoryUpdate.Path);
                virtualDirectory.VirtualPath = directoryUpdate.Path;
            }

            if (directoryUpdate.AccessLevel != null)
            {
                virtualDirectory.AccessLevel = (AccessType)directoryUpdate.AccessLevel;
            }

            await _virtualDirectoryRepository.UpdateAsync(virtualDirectory);
            await _cacheRepository.RemoveAsync(AllDirectoriesCacheKey);
        }

        public async Task MoveToDirectory(Guid directoryToMoveId, Guid newParentDirectoryId)
        {
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, newParentDirectoryId);
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnActiveDirectoryInfo(_session.UserId, directoryToMoveId);
            virtualDirectory.ParentDirectoryId = newParentDirectoryId;
            await _virtualDirectoryRepository.UpdateAsync(virtualDirectory);
            await _cacheRepository.RemoveAsync(AllDirectoriesCacheKey);
        }

        public async Task MoveDirectoryToBin(Guid directoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnActiveDirectoryInfo(_session.UserId, directoryId);
            virtualDirectory.State = false;
            List<VirtualDirectory> directoriesToMoveToBin = new List<VirtualDirectory>() { virtualDirectory };
            List<StoredFile> filesToMoveToBin = new List<StoredFile>();
            await RecursiveUpdateState(directoriesToMoveToBin, filesToMoveToBin, virtualDirectory, false);
            await _virtualDirectoryRepository.UpdateRangeAsync(directoriesToMoveToBin);
            await _storedFileRepository.UpdateRangeAsync(filesToMoveToBin);
            await _cacheRepository.RemoveAsync($"{nameof(FileDetailsDTO)}_owner_{_session.UserId}");
            await _cacheRepository.RemoveAsync(AllDirectoriesCacheKey);
        }

        public async Task RestoreDirectory(Guid directoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnDeletedDirectoryInfo(_session.UserId, directoryId);
            virtualDirectory.State = true;

            if (virtualDirectory.ParentDirectoryId != null)
            {
                virtualDirectory.ParentDirectoryId = (await _virtualDirectoryRepository.SingleOrDefaultAsync(new OwnedRootDirectorySpec(_session.UserId)))!.Id;
            }

            List<VirtualDirectory> directoriesToRestore = new List<VirtualDirectory>() { virtualDirectory };
            List<StoredFile> filesToRestore = new List<StoredFile>();
            await RecursiveUpdateState(directoriesToRestore, filesToRestore, virtualDirectory, false);

            await _virtualDirectoryRepository.UpdateRangeAsync(directoriesToRestore);
            await _storedFileRepository.UpdateRangeAsync(filesToRestore);
            await _cacheRepository.RemoveAsync($"{nameof(FileDetailsDTO)}_owner_{_session.UserId}");
            await _cacheRepository.RemoveAsync(AllDirectoriesCacheKey);
        }

        public async Task DeleteDirectory(Guid directoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnDeletedDirectoryInfo(_session.UserId, directoryId);
            List<VirtualDirectory> directoriesToDelete = new List<VirtualDirectory>() { virtualDirectory };
            List<StoredFile> filesToDelete = new List<StoredFile>();
            await RecursiveDelete(directoriesToDelete, filesToDelete, virtualDirectory);
            await _storedFileRepository.DeleteRangeAsync(filesToDelete);
            await _virtualDirectoryRepository.DeleteRangeAsync(directoriesToDelete);
            foreach (StoredFile storedFile in filesToDelete)
            {
                await _fileSystemRepository.RemoveFromFileSystem(StoredFilePath(storedFile));
            }
        }

        private string StoredFilePath(StoredFile storedFile)
        {
            return $"{storedFile.OwnerId}/{storedFile.Id}";
        }

        private async Task RecursiveUpdateState(List<VirtualDirectory> directoriesToUpdateState, List<StoredFile> filesToUpdateState, VirtualDirectory currentDirectory, bool newState)
        {
            foreach (var childDirectory in currentDirectory.ChildDirectories.Where(cd => cd.State != newState))
            {
                VirtualDirectory? virtualDirectory = await _virtualDirectoryRepository.SingleOrDefaultAsync(new OwnedDirectoriesSpec(_session.UserId, childDirectory.Id, !newState));
                if (virtualDirectory != null)
                {
                    await RecursiveUpdateState(directoriesToUpdateState, filesToUpdateState, virtualDirectory, newState);
                }
                childDirectory.State = newState;
                directoriesToUpdateState.Add(childDirectory);
            }

            foreach (var file in currentDirectory.FilesInDirectory.Where(fid => fid.State != newState))
            {
                file.State = newState;
                filesToUpdateState.Add(file);
            }
        }

        private async Task RecursiveDelete(List<VirtualDirectory> directoriesToDelete, List<StoredFile> filesToDelete, VirtualDirectory currentDirectory)
        {
            foreach (var childDirectory in currentDirectory.ChildDirectories.Where(cd => cd.State == false))
            {
                VirtualDirectory? virtualDirectory = await _virtualDirectoryRepository.SingleOrDefaultAsync(new OwnedDirectoriesSpec(_session.UserId, childDirectory.Id, false));
                if (virtualDirectory != null)
                {
                    await RecursiveDelete(directoriesToDelete, filesToDelete, virtualDirectory);
                }
                directoriesToDelete.Add(childDirectory);
                filesToDelete.AddRange(currentDirectory.FilesInDirectory);
            }
        }

        private string DirectoryDetailsCacheKey(Guid directoryId, string? accessKey)
        {
            return $"{nameof(DirectoryDetailsDTO)}_{directoryId}_{_session.UserId}_{accessKey}";
        }
    }
}
