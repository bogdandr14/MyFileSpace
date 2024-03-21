using AutoMapper;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Infrastructure.Persistence.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Helpers;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ICacheRepository _cacheRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly Session _session;


        public UserService(IMapper mapper, ICacheRepository cacheRepository, IUserRepository userRepository, IVirtualDirectoryRepository virtualDirectoryRepository, Session session)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _session = session;
        }

        #region "Public methods"
        public async Task<bool> CheckEmailAvailable(string email)
        {
            return await _userRepository.FirstOrDefaultAsync(new EmailSpec(email)) == null;
        }

        public async Task<bool> CheckTagNameAvailable(string tagName)
        {
            return await GetCachedUserByTagName(tagName) == null;
        }

        public async Task<UserDetailsDTO> GetUserByTagName(string tagName)
        {
            return _mapper.Map<UserDetailsDTO>(await ValidateTagNameAndRetrieveUser(tagName));
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(Guid userId)
        {
            return _mapper.Map<UserDetailsDTO>(await ValidateAndRetrieveUser(userId));
        }

        public async Task<string> Login(AuthDTO userLogin)
        {
            User user = await ValidateCredentialsAndRetrieveUser(userLogin.Email, userLogin.Password);
            return JsonWebToken.GenerateToken(user);
        }

        public async Task<UserDetailsDTO> Register(AuthDTO userRegister)
        {
            await ValidateEmail(userRegister.Email);
            ValidatePasswordStrength(userRegister.Password);

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
                VirtualPath = "$USER_ROOT"
            };
            await _virtualDirectoryRepository.AddAsync(rootDirectory);
            return _mapper.Map<UserDetailsDTO>(createUser);
        }

        public async Task UpdatePassword(UpdatePasswordDTO updatePassword)
        {
            User user = await ValidateCredentialsAndRetrieveUser(updatePassword.Email, updatePassword.CurrentPassword);

            user.Password = CryptographyUtility.HashKey(updatePassword.NewPassword, out string salt);
            user.Salt = salt;
            user.LastPasswordChange = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUser(Guid userId, UserUpdateDTO userToUpdate)
        {
            User user = await ValidateCredentialsAndRetrieveUser(userToUpdate.Email, userToUpdate.Password);
            await ValidateTagNameUnique(user, userId, userToUpdate.TagName);

            string oldTagName = user.TagName;
            user.TagName = userToUpdate.TagName;

            await _userRepository.UpdateAsync(user);
            await _cacheRepository.RemoveAsync(TagNameCacheKey(oldTagName));
        }
        #endregion

        private async Task<User?> GetCachedUserByTagName(string tagName)
        {
            Func<Task<User?>> userTask = async () => await _userRepository.FirstOrDefaultAsync(new TagNameSpec(tagName));
            return await _cacheRepository.GetAndSetAsync(TagNameCacheKey(tagName), userTask);
        }

        private string TagNameCacheKey(string tagName)
        {
            return $"{nameof(User)}_tagname_{tagName}";
        }

        #region "Validators"
        private async Task ValidateEmail(string email)
        {
            if (!(await CheckEmailAvailable(email)))
            {
                throw new Exception("email already exists");
            }
        }

        private void ValidatePasswordStrength(string password)
        {
            if (password == null)
            {
                throw new Exception("password too weak");
            }
        }

        private async Task<User> ValidateTagNameAndRetrieveUser(string tagName)
        {
            User? user = await GetCachedUserByTagName(tagName);
            if (user == null)
            {
                throw new Exception("User with tagname not found");
            }

            return user;
        }

        private async Task ValidateTagNameUnique(User existingUser, Guid userToUpdateId, string newTagName)
        {
            if (!userToUpdateId.Equals(existingUser.Id) || !userToUpdateId.Equals(_session.UserId))
            {
                throw new Exception("Forbidden, can not update someone else's info");
            }

            if (!newTagName.Equals(existingUser.TagName))
            {
                User? user = await GetCachedUserByTagName(newTagName);
                if (user != null)
                {
                    throw new Exception("tagName already exists");
                }
            }
        }

        private async Task<User> ValidateAndRetrieveUser(Guid userId)
        {
            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("user not found");
            }

            return user;
        }

        private async Task<User> ValidateCredentialsAndRetrieveUser(string email, string passwordToValidate)
        {
            User? user = await _userRepository.FirstOrDefaultAsync(new EmailSpec(email));

            if (user == null)
            {
                throw new Exception("user not found");
            }

            if (_session.IsAuthenticated)
            {
                if (!user.Id.Equals(_session.UserId))
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
        #endregion
    }
}
