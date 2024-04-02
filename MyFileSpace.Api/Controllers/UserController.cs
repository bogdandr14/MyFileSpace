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

        [HttpGet("isAvailable/email/{email:string}")]
        public async Task<ActionResult<bool>> CheckEmailAvailable(string email)
        {
            return await _userService.CheckEmailAvailable(email);
        }

        [HttpGet("isAvailable/tagname/{tagName:string}")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult<bool>> CheckTagNameAvailable(string tagName)
        {
            return await _userService.CheckTagNameAvailable(tagName);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDetailsDTO>> Register(AuthDTO authDTO)
        {
            return await _userService.Register(authDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthDTO authDTO)
        {
            return await _userService.Login(authDTO);
        }

        [HttpGet("tagname/{tagName:string}")]
        public async Task<ActionResult<UserDetailsDTO>> GetUserByTagName(string tagName)
        {
            return await _userService.GetUserByTagName(tagName);
        }

        [HttpGet("{userId:Guid}")]
        public async Task<ActionResult<UserDetailsDTO>> GetUserById(Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpPut]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> UpdateUser(UserUpdateDTO userUpdateDTO)
        {
            await _userService.UpdateUser(userUpdateDTO);
            return Ok();
        }

        [HttpPut("changepassword")]
        [MyFileSpaceAuthorize]
        public async Task<ActionResult> UpdateUserPassword(UpdatePasswordDTO passwordChangeDTO)
        {
            await _userService.UpdatePassword(passwordChangeDTO);
            return Ok();
        }
    }
}
