using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Entities
{
    public class FileAccessKey : IGenericEntity
    {
        public Guid FileId { get; set; }
        public int AccessKeyId { get; set; }

        #region "Navigation properties"
        public virtual AccessKey AccessKey { get; set; }
        public virtual StoredFile AccessibleFile { get; set; }
        #endregion
    }
}
