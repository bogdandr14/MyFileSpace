namespace MyFileSpace.Core.DTOs
{
    public class FileDownloadDTO
    {
        public Stream ContentStream { get; set; }
        public string DownloadName { get; set; }
        public DateTime LastModified { get; set; }
        public string ContentType { get; set; }
    }
}
