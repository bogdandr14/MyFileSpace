using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IFileDataRepository
    {
        public List<FileDTO_old> GetAll();
        public FileDTO_old? GetByGuid(Guid fileGuid);
        public FileDTO_old? GetByName(string fileName);
        public void Add(FileDTO_old file);
        public void Update(FileDTO_old updatedFile);
        public void Delete(Guid fileGuid);
    }
}
