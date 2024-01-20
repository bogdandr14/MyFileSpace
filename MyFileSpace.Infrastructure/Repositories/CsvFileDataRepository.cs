using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Infrastructure.Helpers;
using MyFileSpace.Infrastructure.Models;
using MyFileSpace.SharedKernel.DTOs;
using MyFileSpace.SharedKernel.Repositories;
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
            _config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };
            _storedDataFilePath = configuration.GetConfigValue("DataFilePath");

            if (!Path.Exists(StoredFileDataPath))
            {
                InitialSeed();
            }
        }

        public List<FileDTO> GetAll()
        {
            return GetAllCsv().Select(x => x.ToBase()).ToList();
        }

        public FileDTO? GetByGuid(Guid fileGuid)
        {
            return GetAll().Find(f => f.Guid.Equals(fileGuid));
        }

        public FileDTO? GetByName(string fileName)
        {
            return GetAll().Find(f => f.OriginalName.Equals(fileName));
        }

        public void Add(FileDTO file)
        {
            AddMany(new List<CsvFileData> { CsvFileData.Adapt(file) });
        }

        public void Update(FileDTO updatedFile)
        {
            CsvFileData updatedCsvFile = CsvFileData.Adapt(updatedFile);
            List<CsvFileData> fileDatas = GetAllCsv();
            fileDatas.RemoveAll(f => f.Guid.Equals(updatedCsvFile.Guid));
            fileDatas.Add(updatedCsvFile);

            PersistNewFile(fileDatas, StoredFileDataTempPath);
            File.Delete(StoredFileDataPath);
            File.Move(StoredFileDataTempPath, StoredFileDataPath);
        }

        public void Delete(Guid fileGuid)
        {
            List<CsvFileData> fileDatas = GetAllCsv();
            fileDatas.RemoveAll(f => f.Guid == fileGuid);

            PersistNewFile(fileDatas, StoredFileDataTempPath);
            File.Delete(StoredFileDataPath);
            File.Move(StoredFileDataTempPath, StoredFileDataPath);
        }

        private List<CsvFileData> GetAllCsv()
        {
            using (var reader = new StreamReader(StoredFileDataPath))
            {
                using (var csv = new CsvReader(reader, _config))
                {
                    return csv.GetRecords<CsvFileData>().ToList();
                }
            }
        }

        private void AddMany(List<CsvFileData> files)
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

        private void PersistNewFile(List<CsvFileData> files, string csvPath)
        {
            using (var writer = new StreamWriter(csvPath))
            using (var csvWriter = new CsvWriter(writer, _config))
            {
                csvWriter.NextRecord();
                csvWriter.WriteRecords(files);
            }
        }

        private void PersistExistingFile(List<CsvFileData> files, string csvPath)
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
                csvWriter.NextRecord();
            }
        }
    }
}
