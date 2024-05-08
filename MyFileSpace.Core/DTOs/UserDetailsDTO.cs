namespace MyFileSpace.Core.DTOs
{
    public class UserDetailsDTO : UserDTO
    {
        public List<DirectoryDTO> Directories { get; set; }
        public List<FileDTO> Files { get; set; }
    }
}
