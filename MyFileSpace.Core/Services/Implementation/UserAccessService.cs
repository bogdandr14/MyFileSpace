using AutoMapper;
using MyFileSpace.Caching;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class UserAccessService : IUserAccessService
    {
        private readonly IMapper _mapper;
        private readonly IUserFileAccessRepository _userFileAccessRepository;
        private readonly IUserDirectoryAccessRepository _userDirectoryAccessRepository;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;
        private readonly Session _session;

        public UserAccessService(IMapper mapper,
            IUserFileAccessRepository userFileAccessRepo,
            IUserDirectoryAccessRepository userDirectoryAccessRepo,
            IStoredFileRepository storedFileRepo,
            IVirtualDirectoryRepository virtualDirectoryRepo,
            IUserRepository userRepo,
            ICacheManager cacheManager,
            Session session)
        {
            _mapper = mapper;
            _userFileAccessRepository = userFileAccessRepo;
            _userDirectoryAccessRepository = userDirectoryAccessRepo;
            _storedFileRepository = storedFileRepo;
            _virtualDirectoryRepository = virtualDirectoryRepo;
            _userRepository = userRepo;
            _cacheManager = cacheManager;
            _session = session;
        }

        #region "Public methods"
        public async Task EditAllowedUsers(UserAccessUpdateDTO userAccess)
        {
            userAccess.ObjectType.ValidateObjectType();
            await _userRepository.ValidateExistingUsers(userAccess.AddUserIds);
            await _userRepository.ValidateExistingUsers(userAccess.RemoveUserIds);

            if (userAccess.ObjectType == ObjectType.StoredFile)
            {
                await _storedFileRepository.ValidateOwnActiveFile(_session.UserId, userAccess.ObjectId);
                await _userFileAccessRepository.ValidateAndRetrieveExistingUserFileAccess(userAccess.ObjectId, userAccess.AddUserIds, false);
                List<UserFileAccess> allowedUsersToAdd = userAccess.AddUserIds.Select(userId => new UserFileAccess() { AllowedUserId = userId, FileId = userAccess.ObjectId }).ToList();
                List<UserFileAccess> allowedUsersToRemove = await _userFileAccessRepository.ValidateAndRetrieveExistingUserFileAccess(userAccess.ObjectId, userAccess.RemoveUserIds, true);
                await _userFileAccessRepository.AddRangeAsync(allowedUsersToAdd);
                await _userFileAccessRepository.DeleteRangeAsync(allowedUsersToRemove);
                await _cacheManager.RemoveAsync(userAccess.ObjectId.FileCacheKeyPrefix());
            }
            else if (userAccess.ObjectType == ObjectType.VirtualDirectory)
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, userAccess.ObjectId);
                await _userDirectoryAccessRepository.ValidateAndRetrieveExistingUserDirectoryAccess(userAccess.ObjectId, userAccess.AddUserIds, false);
                List<UserDirectoryAccess> allowedUsersToRemove = await _userDirectoryAccessRepository.ValidateAndRetrieveExistingUserDirectoryAccess(userAccess.ObjectId, userAccess.RemoveUserIds, true);

                List<UserDirectoryAccess> allowedUsersToAdd = userAccess.AddUserIds.Select(userId => new UserDirectoryAccess() { AllowedUserId = userId, DirectoryId = userAccess.ObjectId }).ToList();
                await _userDirectoryAccessRepository.AddRangeAsync(allowedUsersToAdd);
                await _userDirectoryAccessRepository.DeleteRangeAsync(allowedUsersToRemove);
                await _cacheManager.RemoveAsync(userAccess.ObjectId.DirectoryCacheKeyPrefix());
            }
            await _cacheManager.RemoveAsync(GetAllObjectAccessCacheKey(userAccess.ObjectId));
        }

        public async Task<List<UserDTO>> GetAllowedUsers(Guid objectId, ObjectType objectType)
        {
            objectType.ValidateObjectType();

            if (objectType == ObjectType.StoredFile)
            {
                await _storedFileRepository.ValidateOwnActiveFile(_session.UserId, objectId);
            }
            else if (objectType == ObjectType.VirtualDirectory)
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, objectId);
            }

            Func<Task<List<UserDTO>>> allowedUsersTask = async () =>
            {

                return _mapper.Map<List<UserDTO>>(await _userRepository.ListAsync(new UserWithAccessSpec(objectId)));
            };

            return await _cacheManager.GetAndSetAsync(GetAllObjectAccessCacheKey(objectId), allowedUsersTask);
        }
        #endregion

        private string GetAllObjectAccessCacheKey(Guid objectId)
        {
            return $"ObjectAccess_{objectId}";
        }
    }
}
