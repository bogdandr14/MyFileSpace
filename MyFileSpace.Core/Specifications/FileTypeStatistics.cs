using MyFileSpace.SharedKernel.Enums;

namespace MyFileSpace.Core.Specifications
{
    public class FileTypeStatistics
    {
        public AccessType AccessType { get; set; }
        public double SizeMb { get; set; }
        public long Number { get; set; }

        public double Last30DaysAddedSizeMb { get; set; }
        public long Last30DaysAddedNumber { get; set; }
    }
}
