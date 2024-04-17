using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
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
        private readonly ICacheRepository _cacheRepository;
        private readonly Session _session;

        public UserAccessService(IMapper mapper,
            IUserFileAccessRepository userFileAccessRepo,
            IUserDirectoryAccessRepository userDirectoryAccessRepo,
            IStoredFileRepository storedFileRepo,
            IVirtualDirectoryRepository virtualDirectoryRepo,
            IUserRepository userRepo,
            ICacheRepository cacheRepo,
            Session session)
        {
            _mapper = mapper;
            _userFileAccessRepository = userFileAccessRepo;
            _userDirectoryAccessRepository = userDirectoryAccessRepo;
            _storedFileRepository = storedFileRepo;
            _virtualDirectoryRepository = virtualDirectoryRepo;
            _userRepository = userRepo;
            _cacheRepository = cacheRepo;
            _session = session;
        }

        #region "Public methods"
        public async Task AddUserAccess(UserAccessUpdateDTO userAccess)
        {
            userAccess.ObjectType.ValidateObjectType();
            await _userRepository.ValidateExistingUsers(userAccess.UserGuids);

            if (userAccess.ObjectType == ObjectType.StoredFile)
            {
                await _storedFileRepository.ValidateOwnActiveFile(_session.UserId,userAccess.ObjectId);
                await _userFileAccessRepository.ValidateAndRetrieveExistingUserFileAccess(userAccess.ObjectId, userAccess.UserGuids, false);
                List<UserFileAccess> allowedUsersToAdd = userAccess.UserGuids.Select(userId => new UserFileAccess() { AllowedUserId = userId, FileId = userAccess.ObjectId }).ToList();
                await _userFileAccessRepository.AddRangeAsync(allowedUsersToAdd);
            }
            else if (userAccess.ObjectType == ObjectType.VirtualDirectory)
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, userAccess.ObjectId);
                await _userDirectoryAccessRepository.ValidateAndRetrieveExistingUserDirectoryAccess(userAccess.ObjectId, userAccess.UserGuids, false);

                List<UserDirectoryAccess> allowedUsersToAdd = userAccess.UserGuids.Select(userId => new UserDirectoryAccess() { AllowedUserId = userId, DirectoryId = userAccess.ObjectId }).ToList();
                await _userDirectoryAccessRepository.AddRangeAsync(allowedUsersToAdd);
            }
        }

        public async Task RemoveUserAccess(UserAccessUpdateDTO userAccess)
        {
            userAccess.ObjectType.ValidateObjectType();
            await _userRepository.ValidateExistingUsers(userAccess.UserGuids);

            if (userAccess.ObjectType == ObjectType.StoredFile)
            {
                await _storedFileRepository.ValidateOwnActiveFile(_session.UserId, userAccess.ObjectId);

                List<UserFileAccess> allowedUsersToRemove = await _userFileAccessRepository.ValidateAndRetrieveExistingUserFileAccess(userAccess.ObjectId, userAccess.UserGuids, true);
                await _userFileAccessRepository.DeleteRangeAsync(allowedUsersToRemove);
            }
            else if (userAccess.ObjectType == ObjectType.VirtualDirectory)
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId,userAccess.ObjectId);

                List<UserDirectoryAccess> allowedUsersToRemove = await _userDirectoryAccessRepository.ValidateAndRetrieveExistingUserDirectoryAccess(userAccess.ObjectId, userAccess.UserGuids, true);
                await _userDirectoryAccessRepository.DeleteRangeAsync(allowedUsersToRemove);
            }
        }

        public async Task<List<UserPublicInfoDTO>> GetAllowedUsers(Guid objectId, ObjectType objectType)
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

            Func<Task<List<UserPublicInfoDTO>>> allowedUsersTask = async () =>
            {

                return _mapper.Map<List<UserPublicInfoDTO>>(await _userRepository.ListAsync(new UserWithAccessSpec(objectId)));
            };

            return await _cacheRepository.GetAndSetAsync(GetAllObjectAccessCacheKey(objectId), allowedUsersTask);
        }
        #endregion

        private string GetAllObjectAccessCacheKey(Guid objectId)
        {
            return $"ObjectAccess_{objectId}";
        }
    }
}
