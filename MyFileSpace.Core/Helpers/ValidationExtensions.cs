using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;
using System.Text.RegularExpressions;

namespace MyFileSpace.Core.Helpers
{
    internal static class ValidationExtensions
    {
        #region "Simple validators"
        public static void ValidateNotLoggedIn(this Session session)
        {
            if (session.IsAuthenticated)
            {
                throw new ForbiddenException("You can not do this while logged in");
            }
        }

        public static void ValidateObjectType(this ObjectType objectType)
        {
            if (objectType != ObjectType.StoredFile && objectType != ObjectType.VirtualDirectory)
            {
                throw new InvalidException("Object type not allowed");
            }
        }

        public static void ValidatePasswordStrength(this string password)
        {
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");

            if (!validateGuidRegex.IsMatch(password))
            {
                throw new InvalidException("Password must have at least 8 characters, 1 uppercase letter, 1 lowercase letter, 1 digit and 1 special character");
            }
        }

        public static async Task ValidateEmail(this IUserRepository userRepo, string email)
        {
            Regex validateEmailRegex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");

            if (!validateEmailRegex.IsMatch(email))
            {
                throw new InvalidException("The email does not have a valid format");
            }
            if (await userRepo.AnyAsync(new EmailSpec(email)))
            {
                throw new InvalidException("Email already exists");
            }
        }

        public static async Task ValidateFileNameNotInDirectory(this IStoredFileRepository storedFileRepo, Guid directoryId, string fileName)
        {
            if (await storedFileRepo.AnyAsync(new FileNameInDirectorySpec(directoryId, fileName)))
            {
                throw new InvalidException($"File with name ${fileName} already exists in the specified directory!");
            }
        }

        public static async Task ValidateAccessKeyInexistent(this IAccessKeyRepository accessKeyRepo, Guid objectId, ObjectType objectType)
        {
            if (await accessKeyRepo.AnyAsync(new AccessKeySpec(objectId, objectType)))
            {
                throw new InvalidException("An access key already exists for this object");
            }
        }

        public static async Task ValidateDirectoryNotInParentDirectory(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid directoryId, string path)
        {
            if (await virtualDirectoryRepo.AnyAsync(new DirectoryPathInParentDirectorySpec(directoryId, path)))
            {
                throw new InvalidException($"Directory with name ${path} already exists in the parent directory!");
            }
        }

        public static async Task ValidateExistingUsers(this IUserRepository userRepo, List<Guid> userIds)
        {
            List<User> users = await userRepo.ListAsync(new ExistingUsersSpec(userIds));
            if (userIds.Count() != users.Count())
            {
                throw new NotFoundException("One or more of the specified users do not exist");
            }
        }
        #endregion

        #region "Owner validators"
        public static async Task ValidateOwnActiveFile(this IStoredFileRepository storedFileRepo, Guid ownerId, Guid fileId)
        {
            if (!await storedFileRepo.AnyAsync(new OwnedFilesWithDirectoriesSpec(ownerId, fileId)))
            {
                throw new NotFoundException("You do not have the specified file!");
            }
        }

        public static async Task ValidateOwnDirectoryActive(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId, Guid directoryId)
        {
            if (!await virtualDirectoryRepo.AnyAsync(new OwnedDirectoriesSpec(ownerId, directoryId)))
            {
                throw new NotFoundException($"You do not have the specified directory or is deleted!");
            }
        }

        public static async Task ValidateOwnFileEnoughSpace(this IStoredFileRepository storedFileRepo, Guid ownerId, long bytesToAdd)
        {
            List<StoredFile> ownedFiles = await storedFileRepo.ListAsync(new OwnedFilesSpec(ownerId));
            if (ownedFiles.Sum(x => x.SizeInBytes) + bytesToAdd > Constants.MAX_ALLOWED_USER_STORAGE)
            {
                throw new InvalidException("You do not have enough free space to store the file!");
            }
        }
        #endregion

        #region "Validator and retriver"
        public static async Task<User> ValidateCredentialsAndRetrieveUser(this IUserRepository userRepo, string email, string passwordToValidate)
        {
            User? user = await userRepo.FirstOrDefaultAsync(new EmailSpec(email));

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!CryptographyUtility.VerifyKey(passwordToValidate, user.Password, user.Salt))
            {
                throw new InvalidException("Incorrect email or password");
            }

            return user;
        }

        public static async Task<User> ValidateCredentialsAndRetrieveUser(this IUserRepository userRepo, Guid userId, string passwordToValidate)
        {
            User? user = await userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!CryptographyUtility.VerifyKey(passwordToValidate, user.Password, user.Salt))
            {
                throw new InvalidException("Incorrect email or password");
            }

            return user;
        }

        public static async Task<StoredFile> ValidateAndRetrieveFileInfo(this IStoredFileRepository storedFileRepo, Session session, Guid fileId, string? accessKey = null)
        {
            StoredFile? storedFile = await storedFileRepo.SingleOrDefaultAsync(new AllowedFileSpec(fileId, session.UserId));
            if (storedFile != null)
            {
                return storedFile;
            }

            if (accessKey != null)
            {
                storedFile = await storedFileRepo.SingleOrDefaultAsync(new AllowedFileSpec(fileId, accessKey));
                if (storedFile != null)
                {
                    return storedFile;
                }
            }

            throw new NotFoundException("File not found");
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Session session, Guid directoryId, string? accessKey = null)
        {
            VirtualDirectory? virtualDirectory;
            virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new AllowedDirectorySpec(directoryId, session.UserId));
            if (virtualDirectory != null)
            {
                return virtualDirectory;
            }

            if (accessKey != null)
            {
                virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new AllowedDirectorySpec(directoryId, accessKey));
                if (virtualDirectory != null)
                {
                    return virtualDirectory;
                }
            }

            throw new NotFoundException("Directory not found");
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveRootDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId)
        {
            VirtualDirectory? virtualDirectory;
            virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new OwnedDirectoriesSpec(ownerId, true));
            if (virtualDirectory != null)
            {
                return virtualDirectory;
            }

            throw new NotFoundException("Root directory not found");
        }

        public static async Task<List<UserDirectoryAccess>> ValidateAndRetrieveExistingUserDirectoryAccess(this IUserDirectoryAccessRepository userDirectoryAccessRepo, Guid directoryId, List<Guid> userIds, bool shouldExist)
        {
            List<UserDirectoryAccess> existingUserDirectoryAccess = await userDirectoryAccessRepo.ListAsync(new UserDirectoryAccessSpec(directoryId, userIds));
            if (shouldExist && existingUserDirectoryAccess.Count() != userIds.Count())
            {
                throw new InvalidException("One or more users do not have permission to access the specified directory");
            }

            if (!shouldExist && existingUserDirectoryAccess.Any())
            {
                throw new InvalidException("One or more users already have permission to access the specified directory");
            }

            return existingUserDirectoryAccess;
        }

        public static async Task<List<UserFileAccess>> ValidateAndRetrieveExistingUserFileAccess(this IUserFileAccessRepository userFileAccessRepo, Guid fileId, List<Guid> userIds, bool shouldExist)
        {
            List<UserFileAccess> existingUserFileAccess = await userFileAccessRepo.ListAsync(new UserFileAccessSpec(fileId, userIds));
            if (shouldExist && existingUserFileAccess.Count() != userIds.Count())
            {
                throw new InvalidException("One or more users do not have permission to access the specified file");
            }

            if (!shouldExist && existingUserFileAccess.Any())
            {
                throw new InvalidException("One or more users already have permission to access the specified file");
            }

            return existingUserFileAccess;
        }

        public static async Task<User> ValidateAndRetrieveUser(this IUserRepository userRepo, Guid userId)
        {
            User? user = await userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return user;
        }
        #endregion

        #region "Owner validator and retriver"
        public static async Task<AccessKey> ValidateAndRetrieveOwnAccessKey(this IAccessKeyRepository accessKeyRepo, Guid ownerId, Guid objectId, ObjectType objectType)
        {
            AccessKey? accessKey = await accessKeyRepo.SingleOrDefaultAsync(new AccessKeySpec(objectId, objectType, ownerId));

            if (accessKey == null)
            {
                string objectName = objectType == ObjectType.StoredFile ? "file" : "directory";
                throw new NotFoundException($"Access key does not exist for the specified {objectName}");
            }

            return accessKey;
        }

        public static async Task<StoredFile> ValidateAndRetrieveOwnDeletedFileInfo(this IStoredFileRepository storedFileRepo, Session session, Guid fileId)
        {
            StoredFile? storedFile = await storedFileRepo.SingleOrDefaultAsync(new OwnedFilesWithDirectoriesSpec(session.UserId, fileId, true));
            if (storedFile == null)
            {
                throw new NotFoundException("File not in bin or already deleted");
            }
            return storedFile;
        }

        public static async Task<StoredFile> ValidateAndRetrieveOwnActiveFileInfo(this IStoredFileRepository storedFileRepo, Session session, Guid fileId)
        {
            StoredFile? storedFile = await storedFileRepo.SingleOrDefaultAsync(new OwnedFilesWithDirectoriesSpec(session.UserId, fileId));
            if (storedFile == null)
            {
                throw new NotFoundException("File not found or in bin");
            }

            return storedFile;
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveOwnDeletedDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId, Guid directoryId)
        {
            VirtualDirectory? virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new OwnedDirectoriesSpec(ownerId, directoryId, true));
            if (virtualDirectory == null)
            {
                throw new NotFoundException("Directory not in bin or already deleted");
            }

            return virtualDirectory;
        }

        public static async Task<VirtualDirectory> ValidateAndRetrieveOwnActiveDirectoryInfo(this IVirtualDirectoryRepository virtualDirectoryRepo, Guid ownerId, Guid directoryId)
        {
            VirtualDirectory? virtualDirectory = await virtualDirectoryRepo.SingleOrDefaultAsync(new OwnedDirectoriesSpec(ownerId, directoryId, false));
            if (virtualDirectory == null)
            {
                throw new NotFoundException("Directory not found or in bin");
            }

            return virtualDirectory;
        }
        #endregion
    }
}
