using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Core.Helpers
{
    internal static class ValidationExtensions
    {
        #region "Simple validators"

        public static void ValidateObjectType(this ObjectType objectType)
        {
            if (objectType != ObjectType.StoredFile && objectType != ObjectType.VirtualDirectory)
            {
                throw new Exception("can not allow this type of object");
            }
        }

        public static void ValidatePasswordStrength(this string password)
        {
            if (password == null)
            {
                throw new Exception("password too weak");
            }
        }

        public static async Task ValidateEmail(this IUserRepository userRepo, string email)
        {
            if (await userRepo.AnyAsync(new EmailSpec(email)))
            {
                throw new Exception("email already exists");
            }
        }

        public static async Task ValidateFileNameNotInDirectory(this IStoredFileRepository storedFileRepo, Guid directoryId, string fileName)
        {
            if (await storedFileRepo.AnyAsync(new FileNameInDirectorySpec(directoryId, fileName)))
            {
                throw new Exception($"File with name ${fileName} already exists in the specified directory!");
            }
        }

        public static async Task ValidateAccessKeyInexistent(this IAccessKeyRepository accessKeyRepo, Guid objectId, ObjectType objectType)
        {
            if (await accessKeyRepo.AnyAsync(new AccessKeySpec(objectId, objectType)))
            {
                throw new Exception("access key already exists");
            }
        }

        public static async Task ValidateDirectoryNotInParentDirectory(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid directoryId, string path)
        {
            if (await virtualDirectoryRepo.AnyAsync(new DirectoryPathInParentDirectorySpec(directoryId, path)))
            {
                throw new Exception($"Directory with name ${path} already exists in the parent directory!");
            }
        }

        public static async Task ValidateExistingUsers(this IUserRepository userRepo, List<Guid> userIds)
        {
            List<User> users = await userRepo.ListAsync(new ExistingUsersSpec(userIds));
            if (userIds.Count() != users.Count())
            {
                throw new Exception("one or more users do not exist");
            }
        }
        #endregion

        #region "Owner validators"
        public static async Task ValidateOwnActiveFile(this IStoredFileRepository storedFileRepo, Guid ownerId, Guid fileId)
        {
            if (!await storedFileRepo.AnyAsync(new OwnedFilesSpec(ownerId, fileId, true)))
            {
                throw new Exception("User does not have the specified file");
            }
        }

        public static async Task ValidateOwnDirectoryActive(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId, Guid directoryId)
        {
            if (!await virtualDirectoryRepo.AnyAsync(new OwnedDirectoriesSpec(ownerId, directoryId)))
            {
                throw new Exception($"User does not have the specified directory!");
            }
        }
        #endregion

        #region "Validator and retriver"
        public static async Task<User> ValidateCredentialsAndRetrieveUser(this IUserRepository userRepo, Session session, string email, string passwordToValidate)
        {
            User? user = await userRepo.FirstOrDefaultAsync(new EmailSpec(email));

            if (user == null)
            {
                throw new Exception("user not found");
            }

            if (session.IsAuthenticated)
            {
                if (!user.Id.Equals(session.UserId))
                {
                    throw new Exception("Forbidden");
                }
            }

            if (!CryptographyUtility.VerifyKey(passwordToValidate, user.Password, user.Salt))
            {
                throw new Exception("incorrect email or password");
            }

            return user;
        }

        public static async Task<StoredFile> ValidateAndRetrieveFileInfo(this IStoredFileRepository storedFileRepo, Session session, Guid fileId, string? accessKey = null)
        {
            StoredFile? storedFile;
            if (session.IsAuthenticated)
            {
                storedFile = await storedFileRepo.SingleOrDefaultAsync(new AllowedFileSpec(fileId, session.UserId));
                if (storedFile != null)
                {
                    return storedFile;
                }
            }

            if (accessKey != null)
            {
                storedFile = await storedFileRepo.SingleOrDefaultAsync(new AllowedFileSpec(fileId, accessKey));
                if (storedFile != null)
                {
                    return storedFile;
                }
            }

            throw new Exception("file not found");
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Session session, Guid directoryId, string? accessKey = null)
        {
            VirtualDirectory? virtualDirectory;
            if (session.IsAuthenticated)
            {
                virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new AllowedDirectorySpec(directoryId, session.UserId));
                if (virtualDirectory != null)
                {
                    return virtualDirectory;
                }
            }

            if (accessKey != null)
            {
                virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new AllowedDirectorySpec(directoryId, accessKey));
                if (virtualDirectory != null)
                {
                    return virtualDirectory;
                }
            }

            throw new Exception("directory not found");
        }

        public static async Task<List<UserDirectoryAccess>> ValidateAndRetrieveExistingUserDirectoryAccess(this IUserDirectoryAccessRepository userDirectoryAccessRepo, Guid directoryId, List<Guid> userIds, bool shouldExist)
        {
            List<UserDirectoryAccess> existingUserDirectoryAccess = await userDirectoryAccessRepo.ListAsync(new UserDirectoryAccessSpec(directoryId, userIds));
            if (shouldExist && existingUserDirectoryAccess.Count() != userIds.Count())
            {
                throw new Exception("One or more users do not have permission to access the specified file");
            }

            if (!shouldExist && existingUserDirectoryAccess.Any())
            {
                throw new Exception("One or more users already have permission to access the file");
            }

            return existingUserDirectoryAccess;
        }

        public static async Task<List<UserFileAccess>> ValidateAndRetrieveExistingUserFileAccess(this IUserFileAccessRepository userFileAccessRepo, Guid fileId, List<Guid> userIds, bool shouldExist)
        {
            List<UserFileAccess> existingUserFileAccess = await userFileAccessRepo.ListAsync(new UserFileAccessSpec(fileId, userIds));
            if (shouldExist && existingUserFileAccess.Count() != userIds.Count())
            {
                throw new Exception("One or more users do not have permission to access the specified file");
            }

            if (!shouldExist && existingUserFileAccess.Any())
            {
                throw new Exception("One or more users already have permission to access the file");
            }

            return existingUserFileAccess;
        }

        public static async Task<User> ValidateAndRetrieveUser(this IUserRepository userRepo, Guid userId)
        {
            User? user = await userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("user not found");
            }

            return user;
        }
        #endregion

        #region "Owner validator and retriver"
        public static async Task<AccessKey> ValidateAndRetrieveOwnAccessKey(this IAccessKeyRepository accessKeyRepo, Guid ownerId, Guid fileId, ObjectType objectType)
        {
            AccessKey? accessKey = await accessKeyRepo.SingleOrDefaultAsync(new AccessKeySpec(fileId, objectType, ownerId));

            if (accessKey == null)
            {
                throw new Exception("access key does not exist");
            }

            return accessKey;
        }

        public static async Task<StoredFile> ValidateAndRetrieveOwnDeletedFileInfo(this IStoredFileRepository storedFileRepo, Session session, Guid fileId)
        {
            StoredFile? storedFile = await storedFileRepo.SingleOrDefaultAsync(new OwnedFilesSpec(session.UserId, fileId, false));
            if (storedFile == null)
            {
                throw new Exception("File not in bin or already deleted");
            }
            return storedFile;
        }

        public static async Task<StoredFile> ValidateAndRetrieveOwnActiveFileInfo(this IStoredFileRepository storedFileRepo, Session session, Guid fileId)
        {
            StoredFile? storedFile = await storedFileRepo.SingleOrDefaultAsync(new OwnedFilesSpec(session.UserId, fileId, true));
            if (storedFile == null)
            {
                throw new Exception("File not found or deleted");
            }

            return storedFile;
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveOwnDeletedDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId, Guid directoryId)
        {
            VirtualDirectory? virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new OwnedDirectoriesSpec(ownerId, directoryId, false));
            if (virtualDirectory == null)
            {
                throw new Exception("Directory not in bin or already deleted");
            }

            return virtualDirectory;
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveOwnActiveDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId, Guid directoryId)
        {
            VirtualDirectory? virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new OwnedDirectoriesSpec(ownerId, directoryId, true));
            if (virtualDirectory == null)
            {
                throw new Exception("Directory not found or deleted");
            }

            return virtualDirectory;
        }
        #endregion
    }
}
