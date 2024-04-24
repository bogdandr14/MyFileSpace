using Microsoft.AspNetCore.Mvc;
using MyFileSpace.Api.Attributes;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Services;

namespace MyFileSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("isAvailable/email/{email}")]
        public async Task<bool> CheckEmailAvailable(string email)
        {
            return await _userService.CheckEmailAvailable(email);
        }

        [HttpGet("isAvailable/tagname/{tagName}")]
        [MyFileSpaceAuthorize]
        public async Task<bool> CheckTagNameAvailable(string tagName)
        {
            return await _userService.CheckTagNameAvailable(tagName);
        }

        [HttpPost("register")]
        public async Task<UserDetailsDTO> Register(RegisterDTO registerDTO)
        {
            return await _userService.Register(registerDTO);
        }

        [HttpPost("login")]
        public async Task<TokenDTO> Login(AuthDTO authDTO)
        {
            return await _userService.Login(authDTO);
        }

        [HttpGet("tagname/{tagName}")]
        public async Task<UserDetailsDTO> GetUserByTagName(string tagName)
        {
            return await _userService.GetUserByTagName(tagName);
        }

        [HttpGet("{userId:Guid}")]
        public async Task<UserDetailsDTO> GetUserById(Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpPut]
        [MyFileSpaceAuthorize]
        public async Task UpdateUser(UserUpdateDTO userUpdateDTO)
        {
            await _userService.UpdateUser(userUpdateDTO);
        }

        [HttpPut("changepassword")]
        [MyFileSpaceAuthorize]
        public async Task UpdateUserPassword(UpdatePasswordDTO passwordChangeDTO)
        {
            await _userService.UpdatePassword(passwordChangeDTO);
        }
    }
}
