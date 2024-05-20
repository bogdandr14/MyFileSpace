using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Providers;
using Serilog;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IAccessKeyRepository _accessKeyRepository;
        private readonly IUserAccessKeyRepository _userAccessKeyRepository;
        public EmailService(IConfiguration configuration, IUserRepository userRepository, IAccessKeyRepository accessKeyRepository, IUserAccessKeyRepository userAccessKeyRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _accessKeyRepository = accessKeyRepository;
            _userAccessKeyRepository = userAccessKeyRepository;
        }

        public async Task RequestSendMail(MailRequestDTO mailRequest)
        {
            if (mailRequest.Type == UserKeyType.Confirmation)
            {
                await SendConfirmationMail(mailRequest.Email, mailRequest.Language);
            }
            else if (mailRequest.Type == UserKeyType.ResetPassword)
            {
                await SendResetPasswordMail(mailRequest.Email, mailRequest.Language);
            }
        }

        public async Task SendWelcomeMail(string emailAddress, string language)
        {
            User user = await _userRepository.ValidateAndRetrieveUserEmail(emailAddress);
            UserAccessKey userAccessKey = await CreateUserAccessKey(UserKeyType.Confirmation, user.Id);
            EmailContent emailContent;
            if (language.Equals("ro", StringComparison.OrdinalIgnoreCase))
            {
                emailContent = CreateMailContent(Constants.WELCOME_EMAIL_TITLE_RO, Constants.WELCOME_EMAIL_TEMPLATE_RO, userAccessKey.AccessKey.Key);
            }
            else
            {
                emailContent = CreateMailContent(Constants.WELCOME_EMAIL_TITLE_EN, Constants.WELCOME_EMAIL_TEMPLATE_EN, userAccessKey.AccessKey.Key);
            }

            await SendMail(emailAddress, emailContent);
        }

        public async Task SendConfirmationMail(string emailAddress, string language)
        {
            UserAccessKey userAccessKey = await RetrieveValidUserAccessKey(emailAddress, UserKeyType.Confirmation);
            EmailContent emailContent;
            if (language.Equals("ro", StringComparison.OrdinalIgnoreCase))
            {
                emailContent = CreateMailContent(Constants.CONFIRMATION_EMAIL_TITLE_RO, Constants.CONFIRMATION_EMAIL_TEMPLATE_RO, userAccessKey.AccessKey.Key);
            }
            else
            {
                emailContent = CreateMailContent(Constants.CONFIRMATION_EMAIL_TITLE_EN, Constants.CONFIRMATION_EMAIL_TEMPLATE_EN, userAccessKey.AccessKey.Key);
            }

            await SendMail(emailAddress, emailContent);
        }

        public async Task SendResetPasswordMail(string emailAddress, string language)
        {
            UserAccessKey userAccessKey = await RetrieveValidUserAccessKey(emailAddress, UserKeyType.ResetPassword);
            EmailContent emailContent;
            if (language.Equals("ro", StringComparison.OrdinalIgnoreCase))
            {
                emailContent = CreateMailContent(Constants.RESET_PASSWORD_EMAIL_TITLE_RO, Constants.RESET_PASSWORD_EMAIL_TEMPLATE_RO, userAccessKey.AccessKey.Key);
            }
            else
            {
                emailContent = CreateMailContent(Constants.RESET_PASSWORD_EMAIL_TITLE_EN, Constants.RESET_PASSWORD_EMAIL_TEMPLATE_EN, userAccessKey.AccessKey.Key);
            }
            await SendMail(emailAddress, emailContent);
        }

        #region "Helper methods"
        private EmailContent CreateMailContent(string title, string fileTemplateName, string userKey)
        {
            EmailContent emailContent = new EmailContent(title);

            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, @".\EmailTemplates\" + fileTemplateName);
            string sFilePath = Path.GetFullPath(sFile);
            Log.Logger.Information($"Accessing file in path:{sFilePath}");
            string template = File.ReadAllText(sFilePath);

            string clientUrl = _configuration.GetConfigValue("CommunicationEmail:ClientAppUrl");
            string htmlBody = template.Replace("#{CLIENT_URL}", clientUrl);
            emailContent.Html = htmlBody.Replace("#{USER_KEY}", userKey);
            return emailContent;
        }

        private async Task SendMail(string emailAddress, EmailContent emailContent)
        {
            string connectionString = _configuration.GetConfigValue("CommunicationEmail:ConnectionString");
            string senderEmail = _configuration.GetConfigValue("CommunicationEmail:SenderEmail");
            string useDebugRecipient = _configuration.GetConfigValue("CommunicationEmail:UseDebugRecipient");

            if (bool.TryParse(useDebugRecipient, out bool useDebug) && useDebug)
            {
                emailContent = new EmailContent($"{emailContent.Subject} - {emailAddress}") { Html = emailContent.Html };
                emailAddress = _configuration.GetConfigValue("CommunicationEmail:DebugRecipient");
            }

            EmailClient emailClient = new EmailClient(connectionString);
            EmailMessage emailMessage = new EmailMessage(senderEmail, emailAddress, emailContent);

            EmailSendOperation emailSendOperation = emailClient.Send(WaitUntil.Started, emailMessage, CancellationToken.None);
            Console.WriteLine($"MessageId={emailSendOperation.Id}");
        }

        private async Task<UserAccessKey> RetrieveValidUserAccessKey(string email, UserKeyType type)
        {
            User user = await _userRepository.ValidateAndRetrieveUserEmail(email);
            UserAccessKey? userAccessKey = user.UserAccessKeys.SingleOrDefault(uak => uak.Type == type && uak.AccessKey.ExpiresAt.CompareTo(DateTime.UtcNow) > 0);
            if (userAccessKey == null)
            {
                userAccessKey = await CreateUserAccessKey(type, user.Id);
            }

            return userAccessKey;
        }

        private async Task<UserAccessKey> CreateUserAccessKey(UserKeyType userKeyType, Guid userId)
        {
            DateTime expiresAt = DateTime.UtcNow;
            if (userKeyType == UserKeyType.Confirmation)
            {
                expiresAt = DateTime.UtcNow.AddDays(7);
            }

            if (userKeyType == UserKeyType.ResetPassword)
            {
                expiresAt = DateTime.UtcNow.AddDays(1);
            }
            string clearKey = $"{userId}{(int)userKeyType}{expiresAt.ToBinary()}".Replace("-", "").Replace(" ", "");
            Log.Logger.Information($"Creating user access key for :{clearKey}");

            string encryptedKey = await CryptographyUtility.EncryptAsync(clearKey, userId.ToString());
            AccessKey accessKey = await _accessKeyRepository.AddAsync(new AccessKey() { Key = encryptedKey, ExpiresAt = expiresAt });
            UserAccessKey userAccessKey = await _userAccessKeyRepository.AddAsync(new UserAccessKey() { UserId = userId, AccessKeyId = accessKey.Id, Type = userKeyType });
            userAccessKey.AccessKey = accessKey;
            return userAccessKey;
        }
        #endregion
    }
}
