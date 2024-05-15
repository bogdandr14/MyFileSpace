using MyFileSpace.SharedKernel.Entities;

namespace MyFileSpace.Infrastructure.Entities
{
    public class FavoriteFile : IGenericEntity
    {
        public Guid UserId { get; set; }
        public Guid FileId { get; set; }

        #region "Navigation properties"
        public virtual User User { get; set; }
        public virtual StoredFile File { get; set; }
        #endregion
    }
}
