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

        public UserService(IMapper mapper, ICacheRepository cacheRepository, IUserRepository userRepository, IVirtualDirectoryRepository virtualDirectoryRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
        }
        public async Task<bool> CheckEmailAvailable(string email)
        {
            User? user = await _userRepository.FirstOrDefaultAsync(new EmailSpec(email));
            return user == null;
        }

        public async Task<bool> CheckUsernameAvailable(string username)
        {
            User? user = await GetCachedUserByUsername(username);
            return user == null;
        }

        public async Task<UserDetailsDTO> GetUser(string username)
        {
            User? user = await GetCachedUserByUsername(username);
            return _mapper.Map<UserDetailsDTO>(user);
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(Guid userId)
        {
            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("user not found");
            }
            return _mapper.Map<UserDetailsDTO>(user);
        }

        public async Task<string> Login(AuthDTO userLogin)
        {
            User user = await ValidateUserAsync(userLogin.Email, userLogin.Password);
            return JsonWebToken.GenerateToken(user);
        }

        public async Task<UserDetailsDTO> Register(AuthDTO userRegister)
        {
            if (!(await CheckEmailAvailable(userRegister.Email)))
            {
                throw new Exception("email already exists");
            }

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
            User user = await ValidateUserAsync(updatePassword.Email, updatePassword.CurrentPassword);

            user.Password = CryptographyUtility.HashKey(updatePassword.NewPassword, out string salt);
            user.Salt = salt;
            user.LastPasswordChange = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUser(Guid userId, UserUpdateDTO userToUpdate)
        {
            User user = await ValidateUserAsync(userToUpdate.Email, userToUpdate.Password);
            if (!(await CheckUsernameAvailable(userToUpdate.Username)))
            {
                throw new Exception("username already exists");
            }
            string oldUsername = user.Username;
            user.Username = userToUpdate.Username;

            await _userRepository.UpdateAsync(user);
            await _cacheRepository.RemoveAsync(UsernameCacheKey(oldUsername));
        }


        private async Task<User?> GetCachedUserByUsername(string username)
        {
            return await _cacheRepository.GetAndSetAsync(UsernameCacheKey(username), async () => await _userRepository.FirstOrDefaultAsync(new UsernameSpec(username)));
        }

        private string UsernameCacheKey(string username)
        {
            return $"{nameof(User)}_username_{username}";
        }

        private async Task<User> ValidateUserAsync(string email, string passwordToValidate)
        {
            User? user = await _userRepository.FirstOrDefaultAsync(new EmailSpec(email));

            if (user == null)
            {
                throw new Exception("user not found");
            }

            if (!CryptographyUtility.VerifyKey(passwordToValidate, user.Password, user.Salt))
            {
                throw new Exception("incorrect password");
            }

            return user;
        }
    }
}
