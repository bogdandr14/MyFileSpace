using MyFileSpace.Core.DTOs;

namespace MyFileSpace.Core.Services
{
    public interface IEmailService
    {
        Task RequestSendMail(MailRequestDTO mailRequest);

        Task SendWelcomeMail(string emailAddress, string language);

        Task SendConfirmationMail(string emailAddress, string language);

        Task SendResetPasswordMail(string emailAddress, string language);
    }
}
