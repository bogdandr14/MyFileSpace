using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Entities
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
