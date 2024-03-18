using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Persistence.Entities
{
    public class FileLabel : IGenericEntity
    {
        public int LabelId { get; set; }
        public Guid FileId { get; set; }

        #region "Navigation properties"
        public virtual StoredFile File { get; set; }
        public virtual Label Label { get; set; }
        #endregion
    }
}
