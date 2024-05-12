namespace MyFileSpace.Core.DTOs
{
    public class UpdatePasswordDTO
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public bool IsReset { get; set; }
    }
}
