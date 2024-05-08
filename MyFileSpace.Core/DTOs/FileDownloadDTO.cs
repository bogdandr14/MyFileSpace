namespace MyFileSpace.Core.DTOs
{
    public class FileDownloadDTO
    {
        public byte[] ContentStream { get; set; }
        public string DownloadName { get; set; }
        public DateTime LastModified { get; set; }
        public string ContentType { get; set; }
    }
}
