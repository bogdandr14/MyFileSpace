using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.Infrastructure.Repositories
{
    public interface IFileDataRepository
    {
        public List<FileDTO> GetAll();
        public FileDTO? GetByGuid(Guid fileGuid);
        public FileDTO? GetByName(string fileName);
        public void Add(FileDTO file);
        public void Update(FileDTO updatedFile);
        public void Delete(Guid fileGuid);
    }
}
