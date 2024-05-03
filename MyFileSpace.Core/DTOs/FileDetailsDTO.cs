namespace MyFileSpace.Core.DTOs
{
    public class FileDetailsDTO : FileDTO
    {
        public string OwnerTagName { get; set; }

        public string DirectoryName { get; set; }
        public List<string> AllowedUsers { get; set; }
        public KeyAccessDetailsDTO? AccessKey { get; set; }
    }
}
