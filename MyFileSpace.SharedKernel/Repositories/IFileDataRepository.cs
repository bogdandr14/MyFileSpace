using MyFileSpace.SharedKernel.DTOs;

namespace MyFileSpace.SharedKernel.Repositories
{
    public interface IFileDataRepository
    {
        public List<FileData> GetAll();
        public FileData? GetByGuid(Guid fileGuid);
        public FileData? GetByName(string fileName);
        public void Add(FileData file);
        public void Update(FileData updatedFile);
        public void Delete(Guid fileGuid);
    }
}
