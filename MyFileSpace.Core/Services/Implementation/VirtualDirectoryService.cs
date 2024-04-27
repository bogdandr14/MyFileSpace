using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class VirtualDirectoryService : IVirtualDirectoryService
    {
        private readonly IMapper _mapper;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IFileStorageRepository _fileStorageRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly Session _session;

        public VirtualDirectoryService(IMapper mapper, IStoredFileRepository storedFileRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileStorageRepository fileSystemRepository, ICacheRepository cacheRepository, Session session)
        {
            _mapper = mapper;
            _storedFileRepository = storedFileRepository;
            _fileStorageRepository = fileSystemRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _cacheRepository = cacheRepository;
            _session = session;
        }

        #region "Public methods"
        public async Task<List<DirectoryDTO>> GetAllDirectoriesInfo()
        {
            Func<Task<List<DirectoryDTO>>> allDirectoriesTask = async () =>
            {
                List<VirtualDirectory> virtualDirectories = await _virtualDirectoryRepository.ListAsync(new OwnedDirectoriesSpec(_session.UserId));
                return _mapper.Map<List<DirectoryDTO>>(virtualDirectories);
            };
            return await _cacheRepository.GetAndSetAsync(_session.AllDirectoriesCacheKey, allDirectoriesTask);
        }

        public async Task<DirectoryDetailsDTO> GetDirectoryInfo(Guid directoryId, string? accessKey = null)
        {
            Func<Task<DirectoryDetailsDTO>> directoryDetailsTask = async () =>
            {
                VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveDirectoryInfo(_session, directoryId, accessKey);
                DirectoryDetailsDTO directoryDTO = _mapper.Map<DirectoryDetailsDTO>(virtualDirectory);
                if (virtualDirectory.Owner.Id.Equals(_session.UserId))
                {
                    directoryDTO.Files = _mapper.Map<List<FileDTO>>(virtualDirectory.FilesInDirectory.OrderBy(fid => fid.Name));
                    directoryDTO.ChildDirectories = _mapper.Map<List<DirectoryDTO>>(virtualDirectory.ChildDirectories.OrderBy(cd => cd.VirtualPath));
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

                directoryDTO.PathParentDirectories = await RetrieveParentDirectories(virtualDirectory);

                return directoryDTO;
            };
            return await _cacheRepository.GetAndSetAsync(DirectoryDetailsCacheKey(directoryId, accessKey), directoryDetailsTask);

        }

        public async Task<DirectoryDTO> AddDirectory(DirectoryCreateDTO directory)
        {
            if (directory.ParentDirectoryId == null || Guid.Empty.Equals(directory.ParentDirectoryId))
            {
                directory.ParentDirectoryId = (await _virtualDirectoryRepository.SingleOrDefaultAsync(new OwnedRootDirectorySpec(_session.UserId)))!.Id;
            }

            await _virtualDirectoryRepository.ValidateDirectoryNotInParentDirectory(directory.ParentDirectoryId.Value, directory.Name);
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directory.ParentDirectoryId.Value);
            VirtualDirectory virtualDirectory = _mapper.Map<VirtualDirectory>(directory);
            virtualDirectory.OwnerId = _session.UserId;
            VirtualDirectory newDirectory = await _virtualDirectoryRepository.AddAsync(virtualDirectory);
            Task.WaitAll(
                _cacheRepository.RemoveAsync(_session.AllDirectoriesCacheKey),
                _cacheRepository.RemoveAsync(DirectoryDetailsCacheKey(newDirectory.ParentDirectoryId!.Value, null))
            );
            return _mapper.Map<DirectoryDTO>(newDirectory);
        }

        public async Task<DirectoryDTO> UpdateDirectory(DirectoryUpdateDTO directoryUpdate)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveDirectoryInfo(_session, directoryUpdate.DirectoryId);

            if (!string.IsNullOrEmpty(directoryUpdate.Name))
            {
                if (virtualDirectory.ParentDirectoryId == null)
                {
                    throw new InvalidException("Can not change root path");
                }

                await _virtualDirectoryRepository.ValidateDirectoryNotInParentDirectory(virtualDirectory.ParentDirectoryId.Value, directoryUpdate.Name);
                virtualDirectory.VirtualPath = directoryUpdate.Name;
            }

            if (directoryUpdate.AccessLevel != null)
            {
                virtualDirectory.AccessLevel = (AccessType)directoryUpdate.AccessLevel;
            }

            await _virtualDirectoryRepository.UpdateAsync(virtualDirectory);
            await _cacheRepository.RemoveAsync(_session.AllDirectoriesCacheKey);
            VirtualDirectory newDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveDirectoryInfo(_session, directoryUpdate.DirectoryId);
            return _mapper.Map<DirectoryDTO>(newDirectory);
        }

        public async Task MoveDirectory(Guid directoryToMoveId, Guid newParentDirectoryId, bool restore)
        {
            if (restore)
            {
                await RestoreDirectory(directoryToMoveId, newParentDirectoryId);
                await _cacheRepository.RemoveAsync(_session.AllFilesCacheKey);
            }
            else
            {
                await MoveToDirectory(directoryToMoveId, newParentDirectoryId);
            }
            await _cacheRepository.RemoveAsync(_session.AllDirectoriesCacheKey);
        }

        public async Task DeleteDirectory(Guid directoryId, bool permanent)
        {
            if (permanent)
            {
                await DeleteDirectoryPermanently(directoryId);
            }
            else
            {
                await MoveDirectoryToBin(directoryId);
            }

            Task.WaitAll(
                _cacheRepository.RemoveAsync(_session.AllFilesCacheKey),
                _cacheRepository.RemoveAsync(_session.AllDirectoriesCacheKey)
            );
        }
        #endregion

        #region "Private methods"

        private async Task<List<DirectoryDTO>> RetrieveParentDirectories(VirtualDirectory directory)
        {
            List<DirectoryDTO> parentDirectories = new List<DirectoryDTO>();
            while (directory.ParentDirectoryId != null)
            {
                directory = await _virtualDirectoryRepository.GetBySpecAsync(new AllowedDirectorySpec(directory.ParentDirectoryId.Value, _session.UserId));
                if (directory == null)
                {
                    break;
                }
                parentDirectories.Add(_mapper.Map<DirectoryDTO>(directory));
            }

            parentDirectories.Reverse();
            return parentDirectories;
        }
        private async Task MoveToDirectory(Guid directoryToMoveId, Guid newParentDirectoryId)
        {
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, newParentDirectoryId);
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnActiveDirectoryInfo(_session.UserId, directoryToMoveId);
            if (virtualDirectory.ParentDirectoryId == null || virtualDirectory.VirtualPath == Constants.ROOT_DIRECTORY)
            {
                throw new InvalidException("Can not move root directory");
            }
            virtualDirectory.ParentDirectoryId = newParentDirectoryId;
            await _virtualDirectoryRepository.UpdateAsync(virtualDirectory);
        }

        private async Task RestoreDirectory(Guid directoryId, Guid newParentDirectoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnDeletedDirectoryInfo(_session.UserId, directoryId);
            virtualDirectory.IsDeleted = false;
            if (!Guid.Empty.Equals(newParentDirectoryId))
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, newParentDirectoryId);
                virtualDirectory.ParentDirectoryId = newParentDirectoryId;
            }
            else if (virtualDirectory.ParentDirectoryId != null)
            {
                virtualDirectory.ParentDirectoryId = (await _virtualDirectoryRepository.SingleOrDefaultAsync(new OwnedRootDirectorySpec(_session.UserId)))!.Id;
            }

            List<VirtualDirectory> directoriesToRestore = new List<VirtualDirectory>() { virtualDirectory };
            List<StoredFile> filesToRestore = new List<StoredFile>();
            await RecursiveUpdateState(directoriesToRestore, filesToRestore, virtualDirectory, false);

            Task.WaitAll(
                _virtualDirectoryRepository.UpdateRangeAsync(directoriesToRestore),
                _storedFileRepository.UpdateRangeAsync(filesToRestore)
            );
        }

        private async Task MoveDirectoryToBin(Guid directoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnActiveDirectoryInfo(_session.UserId, directoryId);
            virtualDirectory.IsDeleted = true;
            List<VirtualDirectory> directoriesToMoveToBin = new List<VirtualDirectory>() { virtualDirectory };
            List<StoredFile> filesToMoveToBin = new List<StoredFile>();
            await RecursiveUpdateState(directoriesToMoveToBin, filesToMoveToBin, virtualDirectory, true);

            Task.WaitAll(
                _virtualDirectoryRepository.UpdateRangeAsync(directoriesToMoveToBin),
                _storedFileRepository.UpdateRangeAsync(filesToMoveToBin)
            );
        }

        private async Task DeleteDirectoryPermanently(Guid directoryId)
        {
            VirtualDirectory virtualDirectory = await _virtualDirectoryRepository.ValidateAndRetrieveOwnDeletedDirectoryInfo(_session.UserId, directoryId);
            List<VirtualDirectory> directoriesToDelete = new List<VirtualDirectory>() { virtualDirectory };
            List<StoredFile> filesToDelete = new List<StoredFile>();
            await RecursiveDelete(directoriesToDelete, filesToDelete, virtualDirectory);

            Task.WaitAll(
                _storedFileRepository.DeleteRangeAsync(filesToDelete),
                _virtualDirectoryRepository.DeleteRangeAsync(directoriesToDelete)
            );

            IEnumerable<Task> tasks = new List<Task>();
            foreach (StoredFile storedFile in filesToDelete)
            {
                tasks.Append(_fileStorageRepository.RemoveFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString()));
            }

            await Task.WhenAll(tasks);
        }

        private async Task RecursiveUpdateState(List<VirtualDirectory> directoriesToUpdateState, List<StoredFile> filesToUpdateState, VirtualDirectory currentDirectory, bool newDeleteState)
        {
            foreach (var childDirectory in currentDirectory.ChildDirectories.Where(cd => cd.IsDeleted != newDeleteState))
            {
                VirtualDirectory? virtualDirectory = await _virtualDirectoryRepository.SingleOrDefaultAsync(new OwnedDirectoriesSpec(_session.UserId, childDirectory.Id, !newDeleteState));
                if (virtualDirectory != null)
                {
                    await RecursiveUpdateState(directoriesToUpdateState, filesToUpdateState, virtualDirectory, newDeleteState);
                }
                childDirectory.IsDeleted = newDeleteState;
                directoriesToUpdateState.Add(childDirectory);
            }

            foreach (var file in currentDirectory.FilesInDirectory.Where(fid => fid.IsDeleted != newDeleteState))
            {
                file.IsDeleted = newDeleteState;
                filesToUpdateState.Add(file);
            }
        }

        private async Task RecursiveDelete(List<VirtualDirectory> directoriesToDelete, List<StoredFile> filesToDelete, VirtualDirectory currentDirectory)
        {
            foreach (var childDirectory in currentDirectory.ChildDirectories.Where(cd => cd.IsDeleted == true))
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
        #endregion
    }
}
