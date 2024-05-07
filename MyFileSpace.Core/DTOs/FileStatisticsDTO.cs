using MyFileSpace.Core.Specifications;

namespace MyFileSpace.Core.DTOs
{
    public class FileStatisticsDTO
    {
        public List<FileTypeStatistics> FileTypeStatistics { get; set; }
        public double SizeMbPastRetentionTime { get; set; }
        public double CacheUsage { get; set; }
    }
}
