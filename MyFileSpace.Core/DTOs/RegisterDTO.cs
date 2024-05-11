namespace MyFileSpace.Core.DTOs
{
    public class RegisterDTO : AuthDTO
    {
        public string? TagName { get; set; }
        public string Language {  get; set; }
    }
}
