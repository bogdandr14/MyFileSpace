﻿using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;
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
        private readonly Session _session;

        public UserService(IMapper mapper, ICacheRepository cacheRepository, IUserRepository userRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileStorageRepository fileSystemRepository, IStoredFileRepository storedFileRepository, Session session)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _fileSystemRepository = fileSystemRepository;
            _storedFileRepository = storedFileRepository;
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
            UserDetailsDTO userPublicDTO = _mapper.Map<UserDetailsDTO>(_userRepository.ValidateAndRetrieveUser(userId));
            userPublicDTO.Directories = _mapper.Map<List<DirectoryDTO>>((await _virtualDirectoryRepository.ListAsync(new AccessibleUserDirectoriesSpec(userId, _session.UserId))));
            userPublicDTO.Files = _mapper.Map<List<FileDTO>>(await _storedFileRepository.ListAsync(new AccessibleUserFilesSpec(userId, _session.UserId)));
            return userPublicDTO;
        }

        public async Task<TokenDTO> Login(AuthDTO userLogin)
        {
            _session.ValidateNotLoggedIn();
            try
            {
                User user = await _userRepository.ValidateCredentialsAndRetrieveUser(userLogin.Email, userLogin.Password);
                return new TokenDTO() { Token = JsonWebToken.GenerateToken(user) };
            }
            catch
            {
                throw new InvalidException("Email or password are incorrect");
            }
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
            Task.WaitAll(
                _virtualDirectoryRepository.AddAsync(rootDirectory),
                _fileSystemRepository.AddDirectory(user.Id.ToString())
            );
            return _mapper.Map<CurrentUserDTO>(createUser);
        }

        public async Task UpdatePassword(UpdatePasswordDTO updatePassword)
        {
            User user = await _userRepository.ValidateCredentialsAndRetrieveUser(updatePassword.Email, updatePassword.CurrentPassword);

            user.Password = CryptographyUtility.HashKey(updatePassword.NewPassword, out string salt);
            user.Salt = salt;
            user.LastPasswordChange = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUser(UserUpdateDTO userToUpdate)
        {
            User user = await _userRepository.ValidateCredentialsAndRetrieveUser(userToUpdate.Email, userToUpdate.Password);
            await ValidateTagNameUnique(user, userToUpdate.TagName);

            string oldTagName = user.TagName;
            user.TagName = userToUpdate.TagName;

            await _userRepository.UpdateAsync(user);
            await _cacheRepository.RemoveAsync(TagNameCacheKey(oldTagName));
        }
        #endregion

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
            if (!existingUser.Equals(_session.UserId))
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
