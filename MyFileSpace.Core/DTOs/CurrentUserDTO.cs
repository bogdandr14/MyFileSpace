using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.DTOs
{
    public class CurrentUserDTO : UserDetailsDTO
    {
        public string Email { get; set; }
        public RoleType RoleType { get; set; }
        public List<FileDTO> AllowedFiles { get; set; }
        public List<DirectoryDTO> AllowedDirectories { get; set; }
    }
}
