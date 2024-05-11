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
        private readonly IEmailService _emailService;
        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet("search")]
        [MyFileSpaceAuthorize(true)]
        public async Task<UsersFoundDTO> SearchFiles([FromQuery] InfiniteScrollFilter filter)
        {
            return await _userService.SearchUsers(filter);
        }

        [HttpGet("availability/email/{email}")]
        public async Task<bool> CheckEmailAvailable(string email)
        {
            return await _userService.CheckEmailAvailable(email);
        }

        [HttpGet("availability/tagname/{tagName}")]
        public async Task<bool> CheckTagNameAvailable(string tagName)
        {
            return await _userService.CheckTagNameAvailable(tagName);
        }

        [HttpPost("register")]
        public async Task<CurrentUserDTO> Register(RegisterDTO registerDTO)
        {
            return await _userService.Register(registerDTO);
        }

        [HttpPost("login")]
        public async Task<TokenDTO> Login(AuthDTO authDTO)
        {
            return await _userService.Login(authDTO);
        }

        [HttpGet]
        [MyFileSpaceAuthorize]
        public async Task<UserDetailsDTO> GetCurrentUser()
        {
            return await _userService.GetCurrentUser();
        }

        [HttpGet("{userId:Guid}")]
        [MyFileSpaceAuthorize(true)]
        public async Task<UserDetailsDTO> GetUserById(Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpPost("sendMail")]
        public async Task SendMail(MailRequestDTO mailRequest)
        {
            await _emailService.RequestSendMail(mailRequest);
        }

        [HttpPut("confirmMail")]
        public async Task ConfirmEmail([FromBody] string confirmKey)
        {
            await _userService.ConfirmEmail(confirmKey);
        }

        [HttpPut]
        [MyFileSpaceAuthorize]
        public async Task UpdateUser(UserUpdateDTO userUpdateDTO)
        {
            await _userService.UpdateUser(userUpdateDTO);
        }

        [HttpPut("changePassword")]
        [MyFileSpaceAuthorize(true)]
        public async Task UpdateUserPassword(UpdatePasswordDTO passwordChangeDTO)
        {
            await _userService.UpdatePassword(passwordChangeDTO);
        }
    }
}
