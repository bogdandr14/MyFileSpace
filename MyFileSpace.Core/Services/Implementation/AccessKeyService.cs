using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class AccessKeyService : IAccessKeyService
    {
        private readonly IMapper _mapper;
        private readonly IAccessKeyRepository _accessKeyRepository;
        private readonly IDirectoryAccessKeyRepository _directoryAccessKeyRepository;
        private readonly IFileAccessKeyRepository _fileAccessKeyRepository;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly Session _session;

        public AccessKeyService(IMapper mapper,
            IAccessKeyRepository accessKeyRepo,
            IDirectoryAccessKeyRepository directoryAccessKeyRepo,
            IFileAccessKeyRepository fileAccessKeyRepo,
            IStoredFileRepository storedFileRepo,
            IVirtualDirectoryRepository virtualDirectoryRepo,
            Session session)
        {
            _mapper = mapper;
            _accessKeyRepository = accessKeyRepo;
            _directoryAccessKeyRepository = directoryAccessKeyRepo;
            _fileAccessKeyRepository = fileAccessKeyRepo;
            _storedFileRepository = storedFileRepo;
            _virtualDirectoryRepository = virtualDirectoryRepo;
            _session = session;
        }

        #region "Public methods"
        public async Task<string> CreateAccessKey(KeyAccesUpdateDTO keyAccess)
        {
            await ValidateOwnActiveObject(keyAccess.ObjectType, keyAccess.ObjectId);
            await _accessKeyRepository.ValidateAccessKeyInexistent(keyAccess.ObjectId, keyAccess.ObjectType);
            AccessKey accessKey = await CreateBaseAccessKey(keyAccess);

            if (keyAccess.ObjectType == ObjectType.StoredFile)
            {
                await _fileAccessKeyRepository.AddAsync(new FileAccessKey() { AccessKeyId = accessKey.Id, FileId = keyAccess.ObjectId });
            }

            if (keyAccess.ObjectType == ObjectType.VirtualDirectory)
            {
                await _directoryAccessKeyRepository.AddAsync(new DirectoryAccessKey() { AccessKeyId = accessKey.Id, DirectoryId = keyAccess.ObjectId });
            }

            return accessKey.Key;
        }

        public async Task<KeyAccessDetailsDTO> GetAccessKey(ObjectType objectType, Guid objectId)
        {
            AccessKey accessKey = await _accessKeyRepository.ValidateAndRetrieveOwnAccessKey(_session.UserId, objectId, objectType);
            return _mapper.Map<KeyAccessDetailsDTO>(accessKey);
        }

        public async Task DeleteAccessKey(ObjectType objectType, Guid objectId)
        {
            await ValidateOwnActiveObject(objectType, objectId);

            AccessKey accessKey = await _accessKeyRepository.ValidateAndRetrieveOwnAccessKey(_session.UserId,objectId, objectType);

            await _accessKeyRepository.DeleteAsync(accessKey);
        }
        #endregion

        #region "Helper methods"
        private async Task<AccessKey> CreateBaseAccessKey(KeyAccesUpdateDTO keyAccess)
        {
            DateTime expiresAt;
            if (keyAccess.ExpiresIn != null)
            {
                expiresAt = DateTime.UtcNow.Add(keyAccess.ExpiresIn.Value);
            }
            else
            {
                expiresAt = DateTime.MaxValue;
            }
            string clearKey = $"AccessKey_{keyAccess.ObjectType}_{keyAccess.ObjectId}";
            string encryptedKey = Convert.ToHexString(await CryptographyUtility.EncryptAsync(clearKey, _session.UserId.ToString()));
            AccessKey accessKey = await _accessKeyRepository.AddAsync(new AccessKey() { Key = encryptedKey, ExpiresAt = expiresAt });
            return accessKey;
        }

        private async Task ValidateOwnActiveObject(ObjectType objectType, Guid objectId)
        {
            if (objectType == ObjectType.StoredFile)
            {
                await _storedFileRepository.ValidateOwnActiveFile(_session.UserId, objectId);
            }
            else if (objectType == ObjectType.VirtualDirectory)
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, objectId);
            }
            else
            {
                throw new Exception("Can not delete access key for this type of object");
            }
        }
        #endregion
    }
}
