namespace MyFileSpace.Core.DTOs
{
    public class FilesFoundDTO
    {
        public FilesFoundDTO()
        {
            Files = new List<FileDTO>();
        }
        public List<FileDTO> Files { get; set; }
        public int Skipped { get; set; }
        public int Taken { get; set; } = 15;
        public bool AreLast { get; set; }
    }
}
