using CsvHelper;
using CsvHelper.Configuration;
using MyFileSpace.SharedKernel.DTOs;
using System.Globalization;

namespace MyFileSpace.Core.Repositories
{
    internal class CsvFileRepository
    {
        private readonly CsvConfiguration _config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        private readonly string _storedFilesData = "YOUR FILE DIRECTORY\\storedFiles.csv";
        private readonly string _storedFilesDataTemp = "YOUR FILE DIRECTORY\\storedFilesTemp.csv";

        public List<FileData> GetAll()
        {
            // when not cached

            using (var reader = new StreamReader(_storedFilesData))
            {
                using (var csv = new CsvReader(reader, _config))
                {
                    return csv.GetRecords<FileData>().ToList();
                }
            }
        }

        public FileData? GetByGuid(Guid fileGuid)
        {
            return GetAll().Find(f => f.Guid == fileGuid);
        }

        public FileData? GetByName(string fileName)
        {
            return GetAll().Find(f => f.Name == fileName);
        }

        public void Add(FileData file)
        {
            AddMany(new List<FileData> { file });
        }

        private void AddMany(List<FileData> files)
        {
            if (!File.Exists(_storedFilesData))
            {
                PersistNewFile(files, _storedFilesData);
            }
            else
            {
                PersistExistingFile(files, _storedFilesData);
            }
        }

        public void Update(FileData updatedFile)
        {
            List<FileData> fileDatas = GetAll();
            fileDatas.RemoveAll(f => f.Guid == updatedFile.Guid);
            fileDatas.Add(updatedFile);

            PersistNewFile(fileDatas, _storedFilesDataTemp);
            File.Delete(_storedFilesData);
            File.Move(_storedFilesDataTemp, _storedFilesData);
        }

        public void Delete(Guid fileGuid)
        {
            List<FileData> fileDatas = GetAll();
            fileDatas.RemoveAll(f => f.Guid == fileGuid);

            PersistNewFile(fileDatas, _storedFilesDataTemp);
            File.Delete(_storedFilesData);
            File.Move(_storedFilesDataTemp, _storedFilesData);
        }


        private void PersistNewFile(List<FileData> files, string csvPath)
        {
            using (var writer = new StreamWriter(csvPath))
            using (var csvWriter = new CsvWriter(writer, _config))
            {

                csvWriter.WriteHeader<CsvFileMap>();
                csvWriter.NextRecord();
                csvWriter.WriteRecords(files);
            }
        }

        private void PersistExistingFile(List<FileData> files, string csvPath)
        {
            using (var stream = new FileStream(csvPath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer, _config))
            {
                csvWriter.WriteRecords(files);
            }
        }
    }
}
