using CsvHelper.Configuration;
using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Core
{
    public class CsvFileMap : ClassMap<FileData>
    {
        public CsvFileMap()
        {
            Map(m => m.Guid).Name("FileGuid");
            Map(m => m.Name).Name("FileName");
            Map(m => m.Path).Name("FilePath");
            Map(m => m.SizeInBytes).Name("SizeInBytes");
            Map(m => m.ModifiedOn).Name("ModifiedOn");
        }
    }
}
