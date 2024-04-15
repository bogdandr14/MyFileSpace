﻿using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ICacheRepository _cacheRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IFileStorageRepository _fileSystemRepository;
        private readonly Session _session;

        public UserService(IMapper mapper, ICacheRepository cacheRepository, IUserRepository userRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IFileStorageRepository fileSystemRepository, Session session)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _fileSystemRepository = fileSystemRepository;
            _session = session;
        }

        #region "Public methods"
        public async Task<bool> CheckEmailAvailable(string email)
        {
            return await _userRepository.FirstOrDefaultAsync(new EmailSpec(email)) == null;
        }

        public async Task<bool> CheckTagNameAvailable(string tagName)
        {
            return await GetUserByTagNameCached(tagName) == null;
        }

        public async Task<UserDetailsDTO> GetUserByTagName(string tagName)
        {
            return _mapper.Map<UserDetailsDTO>(await ValidateTagNameAndRetrieveUser(tagName));
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(Guid userId)
        {
            return _mapper.Map<UserDetailsDTO>(await _userRepository.ValidateAndRetrieveUser(userId));
        }

        public async Task<string> Login(AuthDTO userLogin)
        {
            _session.ValidateNotLoggedIn();
            User user = await _userRepository.ValidateCredentialsAndRetrieveUser(userLogin.Email, userLogin.Password);
            return JsonWebToken.GenerateToken(user);
        }

        public async Task<UserDetailsDTO> Register(AuthDTO userRegister)
        {
            _session.ValidateNotLoggedIn();
            await _userRepository.ValidateEmail(userRegister.Email);
            userRegister.Password.ValidatePasswordStrength();

            User user = new User()
            {
                Role = RoleType.Customer,
                Email = userRegister.Email,
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
            await _virtualDirectoryRepository.AddAsync(rootDirectory);
            await _fileSystemRepository.AddDirectory(user.Id.ToString());
            return _mapper.Map<UserDetailsDTO>(createUser);
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

        #region "Validators"
        private async Task<User> ValidateTagNameAndRetrieveUser(string tagName)
        {
            User? user = await GetUserByTagNameCached(tagName);
            if (user == null)
            {
                throw new NotFoundException("User with tagname not found");
            }

            return user;
        }

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
