using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Infrastructure.Helpers;
using MyFileSpace.SharedKernel.DTOs;
using System.Globalization;

namespace MyFileSpace.Infrastructure.Repositories
{
    internal class CsvFileDataRepository : IFileDataRepository
    {
        private const string FILE_DATA_NAME = "storedFiles.csv";
        private const string FILE_DATA_TEMP_NAME = "storedFilesTemp.csv";

        private readonly CsvConfiguration _config;
        private readonly string _storedDataFilePath;

        private string StoredFileDataPath { get { return $"{_storedDataFilePath}\\{FILE_DATA_NAME}"; } }
        private string StoredFileDataTempPath { get { return $"{_storedDataFilePath}\\{FILE_DATA_TEMP_NAME}"; } }
        public CsvFileDataRepository(IConfiguration configuration)
        {
            _config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
            _storedDataFilePath = configuration.GetConfigValue("DataFilePath");

            if (!Path.Exists(StoredFileDataPath))
            {
                InitialSeed();
            }
        }
        public List<FileData> GetAll()
        {
            using (var reader = new StreamReader(StoredFileDataPath))
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
            return GetAll().Find(f => f.OriginalName == fileName);
        }

        public void Add(FileData file)
        {
            AddMany(new List<FileData> { file });
        }

        public void Update(FileData updatedFile)
        {
            List<FileData> fileDatas = GetAll();
            fileDatas.RemoveAll(f => f.Guid == updatedFile.Guid);
            fileDatas.Add(updatedFile);

            PersistNewFile(fileDatas, StoredFileDataTempPath);
            File.Delete(StoredFileDataPath);
            File.Move(StoredFileDataTempPath, StoredFileDataPath);
        }

        public void Delete(Guid fileGuid)
        {
            List<FileData> fileDatas = GetAll();
            fileDatas.RemoveAll(f => f.Guid == fileGuid);

            PersistNewFile(fileDatas, StoredFileDataTempPath);
            File.Delete(StoredFileDataPath);
            File.Move(StoredFileDataTempPath, StoredFileDataPath);
        }

        private void AddMany(List<FileData> files)
        {
            if (!File.Exists(StoredFileDataPath))
            {
                PersistNewFile(files, StoredFileDataPath);
            }
            else
            {
                PersistExistingFile(files, StoredFileDataPath);
            }
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

        private void InitialSeed()
        {
            using (var writer = new StreamWriter(StoredFileDataPath))
            using (var csvWriter = new CsvWriter(writer, _config))
            {

                csvWriter.WriteHeader<CsvFileMap>();
                csvWriter.NextRecord();
            }
        }
    }
}
