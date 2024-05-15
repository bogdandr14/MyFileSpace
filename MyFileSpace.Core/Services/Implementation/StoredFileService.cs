using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.Caching;
using MyFileSpace.Core.DTOs;
using MyFileSpace.Core.Helpers;
using MyFileSpace.Core.Specifications;
using MyFileSpace.Core.StorageManager;
using MyFileSpace.Infrastructure.Entities;
using MyFileSpace.Infrastructure.Repositories;
using MyFileSpace.SharedKernel;
using MyFileSpace.SharedKernel.Enums;
using MyFileSpace.SharedKernel.Exceptions;
using MyFileSpace.SharedKernel.Providers;

namespace MyFileSpace.Core.Services.Implementation
{
    internal class StoredFileService : IStoredFileService
    {
        private readonly IMapper _mapper;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IFavoriteFileRepository _favoriteFileRepository;
        private readonly IVirtualDirectoryRepository _virtualDirectoryRepository;
        private readonly IStorageManager _storageManager;
        private readonly ICacheManager _cacheManager;
        private readonly IConfiguration _configuration;
        private readonly Session _session;
        private readonly TimeSpan _binRetentionTime;

        public StoredFileService(IMapper mapper, IConfiguration configuration, IStoredFileRepository storedFileRepository, IFavoriteFileRepository favoriteFileRepository, IVirtualDirectoryRepository virtualDirectoryRepository, IStorageManager storageManager, ICacheManager cacheManager, Session session)
        {
            _mapper = mapper;
            _storedFileRepository = storedFileRepository;
            _storageManager = storageManager;
            _favoriteFileRepository = favoriteFileRepository;
            _virtualDirectoryRepository = virtualDirectoryRepository;
            _cacheManager = cacheManager;
            _configuration = configuration;
            _session = session;

            if (TimeSpan.TryParse(configuration.GetConfigValue("FileStorage:BinRetentionTime"), out TimeSpan lifeSpan))
            {
                _binRetentionTime = lifeSpan;
            }
        }

        #region "Public methods"
        public async Task<MemorySize> GetAllowedStorage()
        {
            if (int.TryParse(_configuration.GetConfigValue("FileStorage:MaxStorageGB"), out int maxStorageGB))
            {
                return new MemorySize() { Scale = "GB", Size = maxStorageGB };
            }
            return new MemorySize() { Scale = "GB", Size = 1 };
        }

        public async Task<FileStatisticsDTO> GetStatistics()
        {
            Func<Task<FileStatisticsDTO>> statisticsTask = async () =>
            {
                FileStatisticsDTO statistics = new FileStatisticsDTO();
                List<StoredFile> allStoredFiles = await _storedFileRepository.ListAsync();

                List<StoredFile> publicStoredFiles = allStoredFiles.Where(f => f.AccessLevel == AccessType.Public && f.IsDeleted == false).ToList();
                statistics.FileTypeStatistics.Add(CreateFileTypeStatistics(publicStoredFiles, AccessType.Public));

                List<StoredFile> restrictedStoredFiles = allStoredFiles.Where(f => f.AccessLevel == AccessType.Restricted && f.IsDeleted == false).ToList();
                statistics.FileTypeStatistics.Add(CreateFileTypeStatistics(restrictedStoredFiles, AccessType.Restricted));

                List<StoredFile> privateStoredFiles = allStoredFiles.Where(f => f.AccessLevel == AccessType.Private && f.IsDeleted == false).ToList();
                statistics.FileTypeStatistics.Add(CreateFileTypeStatistics(privateStoredFiles, AccessType.Private));

                List<StoredFile> deletedStoredFiles = allStoredFiles.Where(f => f.IsDeleted == true).ToList();
                statistics.FileTypeStatistics.Add(CreateFileTypeStatistics(deletedStoredFiles, AccessType.None));

                statistics.SizeMbPastRetentionTime = allStoredFiles.Where(f => f.IsDeleted == true && f.ModifiedAt.Add(_binRetentionTime).CompareTo(DateTime.UtcNow) < 0).Sum(f => ((double)f.SizeInBytes) / 1024 / 1024);
                return statistics;
            };

            return await _cacheManager.GetAndSetAsync("statistics", statisticsTask);
        }

        public async Task DeletePastBinRetention()
        {
            List<StoredFile> allStoredFiles = await _storedFileRepository.ListAsync();
            IEnumerable<StoredFile> filesToDelete = allStoredFiles.Where(f => f.IsDeleted == true && f.ModifiedAt.Add(_binRetentionTime).CompareTo(DateTime.UtcNow) < 0).ToList();
            List<VirtualDirectory> allDirectories = await _virtualDirectoryRepository.ListAsync();
            IEnumerable<VirtualDirectory> directoriesToDelete = allDirectories.Where(d => d.IsDeleted == true && d.ModifiedAt.Add(_binRetentionTime).CompareTo(DateTime.UtcNow) < 0).ToList();
            await _storedFileRepository.DeleteRangeAsync(filesToDelete);
            await _virtualDirectoryRepository.DeleteRangeAsync(directoriesToDelete);
            await _cacheManager.RemoveAsync("statistics");
        }

        public async Task<FilesFoundDTO> SearchFiles(InfiniteScrollFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Name))
            {
                filter.Name = " ";
            }
            //increase to check if last files
            filter.Take++;
            List<StoredFile> storedFiles;
            if (_session.IsAuthenticated)
            {
                storedFiles = await _storedFileRepository.ListAsync(new SearchFilesSpec(filter, _session.UserId));
            }
            else
            {
                storedFiles = await _storedFileRepository.ListAsync(new SearchFilesSpec(filter));
            }

            FilesFoundDTO filesFound = new FilesFoundDTO();
            filesFound.Skipped = filter.Skip;
            filesFound.AreLast = storedFiles.Count < filter.Take;
            if (!filesFound.AreLast)
            {
                storedFiles.RemoveAt(storedFiles.Count - 1);
            }
            filesFound.Taken = storedFiles.Count;
            filesFound.Items = _mapper.Map<List<FileDTO>>(storedFiles);

            return filesFound;
        }

        public async Task<List<FileDTO>> GetAllFilesInfo(bool deletedFiles)
        {
            Func<Task<List<FileDTO>>> allFilesTask = async () =>
            {
                List<StoredFile> storedFiles = await _storedFileRepository.ListAsync(new OwnedFilesWithDirectoriesSpec(_session.UserId));
                return _mapper.Map<List<FileDTO>>(storedFiles);
            };

            return (await _cacheManager.GetAndSetAsync(_session.AllFilesCacheKey, allFilesTask)).Where(f => f.IsDeleted == deletedFiles).ToList();
        }

        public async Task<FileDetailsDTO> GetFileInfo(Guid fileId, string? accessKey = null)
        {
            Func<Task<FileDetailsDTO>> fileInfoTask = async () =>
            {
                StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey);
                FileDetailsDTO fileDetailsDTO = _mapper.Map<FileDetailsDTO>(storedFile);
                if (fileDetailsDTO.OwnerId.Equals(_session.UserId) && storedFile.FileAccessKey != null)
                {
                    fileDetailsDTO.AccessKey = _mapper.Map<KeyAccessDetailsDTO>(storedFile.FileAccessKey.AccessKey);
                    if (fileDetailsDTO.AccessKey.ExpiresAt == DateTime.MaxValue)
                    {
                        fileDetailsDTO.AccessKey.ExpiresAt = null;
                    }
                }
                return fileDetailsDTO;
            };

            return await _cacheManager.GetAndSetAsync(fileId.FileCacheKey(_session, accessKey), fileInfoTask);
        }

        public async Task<FileDownloadDTO> DownloadFile(Guid fileId, string? accessKey = null)
        {
            // validates the user has access to the file
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId, accessKey);
            FileDownloadDTO fileDownloadDTO = _mapper.Map<FileDownloadDTO>(storedFile);
            fileDownloadDTO.ContentStream = await _storageManager.ReadFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString());
            return fileDownloadDTO;
        }

        public async Task<FileDTO> UploadNewFile(IFormFile file, Guid directoryId, AccessType accessLevel)
        {
            await _storedFileRepository.ValidateFileNameNotInDirectory(directoryId, file.FileName);
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);

            await _storedFileRepository.ValidateOwnFileEnoughSpace(_session.UserId, file.Length, RetrieveMaxAllowedStorage());

            StoredFile fileToStore = new StoredFile()
            {
                OwnerId = _session.UserId,
                DirectorId = directoryId,
                Name = file.FileName,
                AccessLevel = accessLevel,
                SizeInBytes = file.Length,
                ContentType = file.ContentType
            };
            StoredFile storedFile = await _storedFileRepository.AddAsync(fileToStore);
            Task.WaitAll(
                _storageManager.UploadFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString(), file),
                _cacheManager.RemoveAsync(_session.AllFilesCacheKey)
            );

            return _mapper.Map<FileDTO>(storedFile);
        }

        public async Task<FileDTO> UpdateFileInfo(FileUpdateDTO fileUpdate)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileUpdate.FileId);
            if (!string.IsNullOrEmpty(fileUpdate.Name) && storedFile.Name != fileUpdate.Name)
            {
                await _storedFileRepository.ValidateFileNameNotInDirectory(storedFile.DirectorId, fileUpdate.Name);
                storedFile.Name = fileUpdate.Name;
            }

            if (fileUpdate.AccessLevel != (int)AccessType.None)
            {
                storedFile.AccessLevel = (AccessType)fileUpdate.AccessLevel;
            }

            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheManager.RemoveAsync(_session.AllFilesCacheKey);
            await _cacheManager.RemoveByPrefixAsync(fileUpdate.FileId.FileCacheKeyPrefix());
            return _mapper.Map<FileDTO>(await _storedFileRepository.GetByIdAsync(fileUpdate.FileId));
        }

        public async Task<FileDTO> UploadExistingFile(IFormFile file, Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId);
            if (file.ContentType != storedFile.ContentType)
            {
                throw new InvalidException("can not change the content type of the file");
            }
            await _storedFileRepository.ValidateOwnFileEnoughSpace(_session.UserId, file.Length - storedFile.SizeInBytes, RetrieveMaxAllowedStorage());

            await _storageManager.UploadFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString(), file);

            storedFile.Name = file.FileName;
            storedFile.SizeInBytes = file.Length;
            await _storedFileRepository.UpdateAsync(storedFile);
            await _cacheManager.RemoveAsync(_session.AllFilesCacheKey);
            await _cacheManager.RemoveByPrefixAsync(fileId.FileCacheKeyPrefix());
            return _mapper.Map<FileDTO>(await _storedFileRepository.GetByIdAsync(fileId));
        }

        public async Task MoveFile(Guid fileId, Guid directoryId, bool restore)
        {
            if (restore)
            {
                await RestoreFile(fileId, directoryId);
            }
            else
            {
                await MoveToDirectory(fileId, directoryId);
            }

            await _cacheManager.RemoveAsync(_session.AllFilesCacheKey);
            await _cacheManager.RemoveByPrefixAsync(fileId.FileCacheKeyPrefix());
        }

        public async Task AddToFavorites(Guid fileId)
        {
            await _storedFileRepository.ValidateAndRetrieveFileInfo(_session, fileId);
            await _favoriteFileRepository.ValidateFileNotFavorite(fileId, _session.UserId);
            FavoriteFile favoriteFile = new FavoriteFile() { FileId = fileId, UserId = _session.UserId };
            await _favoriteFileRepository.AddAsync(favoriteFile);
            await _cacheManager.RemoveAsync(fileId.FileCacheKey(_session, null));
            await _cacheManager.RemoveAsync(_session.AllFilesCacheKey);
        }

        public async Task RemoveFromFavorites(Guid fileId)
        {
            FavoriteFile favoriteFile = await _favoriteFileRepository.ValidateAndRetrieveFavoriteFile(fileId, _session.UserId);
            await _favoriteFileRepository.DeleteAsync(favoriteFile);
            await _cacheManager.RemoveAsync(fileId.FileCacheKey(_session, null));
            await _cacheManager.RemoveAsync(_session.AllFilesCacheKey);
        }

        public async Task DeleteFile(Guid fileId, bool permanent)
        {
            if (permanent)
            {
                await DeleteFilePermanently(fileId);
            }
            else
            {
                await MoveFileToBin(fileId);
            }
            await _cacheManager.RemoveAsync(_session.AllFilesCacheKey);
            await _cacheManager.RemoveByPrefixAsync(fileId.FileCacheKeyPrefix());
        }
        #endregion

        #region "Private methods"
        private long RetrieveMaxAllowedStorage()
        {
            long maxAllowedSize = Constants.MAX_ALLOWED_USER_STORAGE;
            if (int.TryParse(_configuration.GetConfigValue("FileStorage:MaxStorageGB"), out int maxStorageGB))
            {
                maxAllowedSize = maxStorageGB * 1024 * 1024 * 1024;
            }

            return maxAllowedSize;
        }
        private FileTypeStatistics CreateFileTypeStatistics(List<StoredFile> storedFiles, AccessType accessType)
        {
            return new FileTypeStatistics()
            {
                AccessLevel = (int)accessType,
                Number = storedFiles.Count(),
                SizeMb = storedFiles.Sum(f => ((double)f.SizeInBytes) / 1024 / 1024),
                Last30DaysAddedNumber = storedFiles.Count(f => f.CreatedAt.AddDays(30).CompareTo(DateTime.UtcNow) > 0),
                Last30DaysAddedSizeMb = storedFiles.Where(f => f.CreatedAt.AddDays(30).CompareTo(DateTime.UtcNow) > 0).Sum(f => ((double)f.SizeInBytes) / 1024 / 1024),
            };
        }
        private async Task RestoreFile(Guid fileId, Guid directoryId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            storedFile.IsDeleted = false;
            if (!Guid.Empty.Equals(directoryId))
            {
                await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);
                storedFile.DirectorId = directoryId;
            }
            else if (storedFile.Directory.IsDeleted == true)
            {
                VirtualDirectory directory = await _virtualDirectoryRepository.ValidateAndRetrieveRootDirectoryInfo(_session.UserId);
                storedFile.DirectorId = directory.Id;
            }
            await _storedFileRepository.UpdateAsync(storedFile);
        }

        private async Task MoveToDirectory(Guid fileId, Guid directoryId)
        {
            await _virtualDirectoryRepository.ValidateOwnDirectoryActive(_session.UserId, directoryId);
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnActiveFileInfo(_session, fileId);
            storedFile.DirectorId = directoryId;
            await _storedFileRepository.UpdateAsync(storedFile);
        }

        private async Task MoveFileToBin(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnActiveFileInfo(_session, fileId);
            storedFile.IsDeleted = true;
            await _storedFileRepository.UpdateAsync(storedFile);
        }

        private async Task DeleteFilePermanently(Guid fileId)
        {
            StoredFile storedFile = await _storedFileRepository.ValidateAndRetrieveOwnDeletedFileInfo(_session, fileId);
            await _storageManager.RemoveFile(storedFile.OwnerId.ToString(), storedFile.Id.ToString());
            await _storedFileRepository.DeleteAsync(storedFile);
        }
        #endregion
    }
}
