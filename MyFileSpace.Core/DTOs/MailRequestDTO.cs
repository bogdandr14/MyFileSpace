using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class MailRequestDTO
    {
        public string Email { get; set; }
        public UserKeyType Type { get; set; }
        public string Language {  get; set; }
    }
}
