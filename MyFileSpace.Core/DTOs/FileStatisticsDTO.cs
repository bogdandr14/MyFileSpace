using MyFileSpace.Core.Specifications;

namespace MyFileSpace.Core.DTOs
{
    public class FileStatisticsDTO
    {
        public FileStatisticsDTO()
        {
            FileTypeStatistics = new List<FileTypeStatistics>();
        }
        public List<FileTypeStatistics> FileTypeStatistics { get; set; }
        public double SizeMbPastRetentionTime { get; set; }
    }
}
