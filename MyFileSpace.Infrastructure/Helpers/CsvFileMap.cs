using CsvHelper.Configuration;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Infrastructure.Helpers
{
    internal class CsvFileMap : ClassMap<FileData>
    {
        public CsvFileMap()
        {
            Map(m => m.Guid).Name("FileGuid");
            Map(m => m.OriginalName).Name("FileName");
            Map(m => m.SizeInBytes).Name("SizeInBytes");
            Map(m => m.ModifiedOn).Name("ModifiedOn");
        }
    }
}
