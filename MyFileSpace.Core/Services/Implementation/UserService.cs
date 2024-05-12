using AutoMapper;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;
using MyFileSpace.SharedKernel.Providers;
using System.Text.RegularExpressions;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ICacheRepository _cacheRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IFileStorageRepository _fileSystemRepository;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUserAccessKeyRepository _userAccessKeyRepository;
        private readonly IAccessKeyRepository _accessKeyRepository;
        private readonly Session _session;

        public UserService(IMapper mapper, IConfiguration configuration, ICacheRepository cacheRepository, IUserRepository userRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileStorageRepository fileSystemRepository, IStoredFileRepository storedFileRepository, IEmailService emailService, IUserAccessKeyRepository userAccessKeyRepository, IAccessKeyRepository accessKeyRepository, Session session)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _fileSystemRepository = fileSystemRepository;
            _storedFileRepository = storedFileRepository;
            _emailService = emailService;
            _userAccessKeyRepository = userAccessKeyRepository;
            _accessKeyRepository = accessKeyRepository;
            _session = session;
        }

        #region "Public methods"
        public async Task<UsersFoundDTO> SearchUsers(InfiniteScrollFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Name))
            {
                filter.Name = " ";
            }
            //increase to check if last files
            filter.Take++;
            List<User> users = await _userRepository.ListAsync(new SearchUsersSpec(filter, _session.UserId));

            UsersFoundDTO usersFound = new UsersFoundDTO();
            usersFound.Skipped = filter.Skip;
            usersFound.AreLast = users.Count < filter.Take;
            if (!usersFound.AreLast)
            {
                users.RemoveAt(users.Count - 1);
            }
            usersFound.Taken = users.Count;
            usersFound.Items = _mapper.Map<List<UserDTO>>(users);

            return usersFound;
        }
        public async Task<bool> CheckEmailAvailable(string email)
        {
            return await _userRepository.FirstOrDefaultAsync(new EmailSpec(email)) == null;
        }

        public async Task<bool> CheckTagNameAvailable(string tagName)
        {
            return await GetUserByTagNameCached(tagName) == null;
        }

        public async Task<CurrentUserDTO> GetCurrentUser()
        {
            CurrentUserDTO userDetailsDTO = _mapper.Map<CurrentUserDTO>(await _userRepository.ValidateAndRetrieveUser(_session.UserId));
            Func<Task<List<FileDTO>>> allFilesTask = async () =>
            {
                List<StoredFile> storedFiles = await _storedFileRepository.ListAsync(new OwnedFilesWithDirectoriesSpec(_session.UserId));
                return _mapper.Map<List<FileDTO>>(storedFiles);
            };

            Func<Task<List<DirectoryDTO>>> allDirectoriesTask = async () =>
            {
                List<VirtualDirectory> virtualDirectories = await _virtualDirectoryRepository.ListAsync(new OwnedDirectoriesSpec(_session.UserId));
                return _mapper.Map<List<DirectoryDTO>>(virtualDirectories);
            };

            userDetailsDTO.Files = await _cacheRepository.GetAndSetAsync(_session.AllFilesCacheKey, allFilesTask);
            userDetailsDTO.Directories = await _cacheRepository.GetAndSetAsync(_session.AllDirectoriesCacheKey, allDirectoriesTask);
            userDetailsDTO.AllowedDirectories = _mapper.Map<List<DirectoryDTO>>((await _virtualDirectoryRepository.ListAsync(new AccessibleUserDirectoriesSpec(_session.UserId))));
            userDetailsDTO.AllowedFiles = _mapper.Map<List<FileDTO>>(await _storedFileRepository.ListAsync(new AccessibleUserFilesSpec(_session.UserId)));

            return userDetailsDTO;
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(Guid userId)
        {
            UserDetailsDTO userPublicDTO = _mapper.Map<UserDetailsDTO>(await _userRepository.ValidateAndRetrieveUser(userId));
            userPublicDTO.Directories = _mapper.Map<List<DirectoryDTO>>((await _virtualDirectoryRepository.ListAsync(new AccessibleUserDirectoriesSpec(userId, _session.UserId))));
            userPublicDTO.Files = _mapper.Map<List<FileDTO>>(await _storedFileRepository.ListAsync(new AccessibleUserFilesSpec(userId, _session.UserId)));
            return userPublicDTO;
        }

        public async Task<TokenDTO> Login(AuthDTO userLogin)
        {
            _session.ValidateNotLoggedIn();
            User user;
            try
            {
                user = await _userRepository.ValidateCredentialsAndRetrieveUser(userLogin.Email, userLogin.Password);
            }
            catch
            {
                throw new InvalidException("Email or password are incorrect");
            }

            if (!user.IsConfirmed)
            {
                if (!bool.TryParse(_configuration.GetConfigValue("DisableConfirmation"), out bool disableConfirmation) || !disableConfirmation)
                {
                    throw new InvalidException("Email is not confirmed");
                }
            }
            return new TokenDTO() { Token = JsonWebToken.GenerateToken(user) };
        }

        public async Task ConfirmEmail(string confirmKey)
        {
            User user = await _userRepository.ValidateConfirmKeyAndRetrieveUser(confirmKey);
            user.IsConfirmed = true;
            await _userRepository.UpdateAsync(user);
            await RemoveUserAccessKey(user.Id, confirmKey);
        }

        public async Task<CurrentUserDTO> Register(RegisterDTO userRegister)
        {
            _session.ValidateNotLoggedIn();
            await _userRepository.ValidateEmail(userRegister.Email);
            userRegister.Password.ValidatePasswordStrength();
            if (string.IsNullOrEmpty(userRegister.TagName))
            {
                userRegister.TagName = await GenerateTagName(userRegister.Email);
            }
            else if (await GetUserByTagNameCached(userRegister.TagName) != null)
            {
                throw new InvalidException("TagName already exists");
            }

            User user = new User()
            {
                Role = RoleType.Customer,
                Email = userRegister.Email,
                TagName = userRegister.TagName,
                Password = CryptographyUtility.HashKey(userRegister.Password, out string salt)
            };
            user.Salt = salt;
            User createUser = await _userRepository.AddAsync(user);

            VirtualDirectory rootDirectory = new VirtualDirectory()
            {
                AccessLevel = AccessType.Private,
                OwnerId = createUser.Id,
                VirtualPath = Constants.ROOT_DIRECTORY,
            };

            List<Task> tasks = new List<Task>() {
                _virtualDirectoryRepository.AddAsync(rootDirectory),
                _fileSystemRepository.AddDirectory(user.Id.ToString()) };

            bool.TryParse(_configuration.GetConfigValue("DisableConfirmation"), out bool disableConfirmation);
            if (!disableConfirmation)
            {
                tasks.Add(_emailService.SendWelcomeMail(userRegister.Email, userRegister.Language));
            }
            Task.WaitAll(tasks.ToArray());
            return _mapper.Map<CurrentUserDTO>(createUser);
        }

        public async Task UpdatePassword(UpdatePasswordDTO updatePassword)
        {
            User user;
            if (updatePassword.IsReset)
            {
                user = await _userRepository.ValidateResetKeyAndRetrieveUser(updatePassword.Email, updatePassword.CurrentPassword);
            }
            else if (_session.IsAuthenticated)
            {
                user = await _userRepository.ValidateCredentialsAndRetrieveUser(updatePassword.Email, updatePassword.CurrentPassword);
            }
            else
            {
                throw new UnauthorizedException("Cannot change password when not logged in");
            }

            user.Password = CryptographyUtility.HashKey(updatePassword.NewPassword, out string salt);
            user.Salt = salt;
            user.LastPasswordChange = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            if (updatePassword.IsReset)
            {
                await RemoveUserAccessKey(user.Id, updatePassword.CurrentPassword);
            }
        }

        public async Task UpdateUser(UserUpdateDTO userToUpdate)
        {
            User user = await _userRepository.ValidateCredentialsAndRetrieveUser(_session.UserId, userToUpdate.Password);
            await ValidateTagNameUnique(user, userToUpdate.TagName);

            string oldTagName = user.TagName;
            user.TagName = userToUpdate.TagName;

            await _userRepository.UpdateAsync(user);
            await _cacheRepository.RemoveAsync(TagNameCacheKey(oldTagName));
        }
        #endregion

        private async Task RemoveUserAccessKey(Guid userId, string accessKey)
        {
            UserAccessKey? userAccessKey = await _userAccessKeyRepository.SingleOrDefaultAsync(new UserAccessKeySpec(userId, accessKey));

            if (userAccessKey != null)
            {
                await _userAccessKeyRepository.DeleteAsync(userAccessKey);
                await _accessKeyRepository.DeleteAsync(userAccessKey.AccessKey);
            }
        }

        private async Task<User?> GetUserByTagNameCached(string tagName)
        {
            Func<Task<User?>> userTask = async () => await _userRepository.FirstOrDefaultAsync(new TagNameSpec(tagName));
            return await _cacheRepository.GetAndSetAsync(TagNameCacheKey(tagName), userTask);
        }

        private string TagNameCacheKey(string tagName)
        {
            return $"{nameof(User)}_tagname_{tagName}";
        }

        private async Task<string> GenerateTagName(string email)
        {
            Regex emailExtraction = new Regex("(?<target>[^,]+)@([\\w-]+\\.)+[\\w-]{2,4}$");
            Group? tokenGroup = emailExtraction.Match(email).Groups["target"];
            string firstPart = tokenGroup.Value;
            Random rand = new Random();

            string generatedTagName;
            do
            {
                int nrOfDigits = rand.Next(0, 3);
                if (firstPart.Length > 8)
                {
                    int nrOfEmailCharacters = rand.Next(8, (firstPart.Length + 8) / 2);
                    generatedTagName = firstPart.Substring(0, nrOfEmailCharacters);
                }
                else
                {
                    generatedTagName = firstPart;
                    while (generatedTagName.Length < 8)
                    {
                        generatedTagName += (char)('a' + rand.Next(0, 25));
                    }
                }

                while (nrOfDigits > 0)
                {
                    generatedTagName = $"{generatedTagName}{rand.Next(0, 9)}";
                    --nrOfDigits;
                }
            } while (!await CheckTagNameAvailable(generatedTagName));

            return generatedTagName;
        }
        #region "Validators"

        private async Task ValidateTagNameUnique(User existingUser, string newTagName)
        {
            if (!existingUser.Id.Equals(_session.UserId))
            {
                throw new InvalidException("Can not modify other users' data");
            }

            if (!newTagName.Equals(existingUser.TagName))
            {
                User? user = await GetUserByTagNameCached(newTagName);
                if (user != null)
                {
                    throw new InvalidException("TagName already exists");
                }
            }
        }
        #endregion
    }
}
