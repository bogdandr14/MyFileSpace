using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyFileSpace.SharedKernel.Providers;

namespace MyFileSpace.Infrastructure.Repositories.Implementation
{
    internal class AzureStorageRepository : IFileStorageRepository
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorageRepository(IConfiguration configuration, ISecretProvider secretProvider)
        {
            string storageAccessKey = secretProvider.GetSecret("FileStorage:AzureBlobStorageAccessKey").GetAwaiter().GetResult();
            string azureStorageName = configuration.GetConfigValue("FileStorage:AzureBlobStorageName");
            _blobServiceClient = new BlobServiceClient(
                new Uri($"https://{azureStorageName}.blob.core.windows.net"),
                new StorageSharedKeyCredential(azureStorageName, storageAccessKey));
        }

        public async Task AddDirectory(string directoryName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(directoryName);
            await blobContainerClient.CreateAsync();
        }

        public async Task UploadFile(string directory, string fileName, IFormFile file)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(directory);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            BlobContentInfo result = await blobClient.UploadAsync(file.OpenReadStream(), blobHttpHeaders);

            if (result is null)
            {
                throw new Exception("error on upload file");
            };
        }

        public async Task<Stream> ReadFile(string directory, string fileName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(directory);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            Response<BlobDownloadResult> blobDownloadResult = await blobClient.DownloadContentAsync();

            if (blobDownloadResult.GetRawResponse().IsError)
            {
                throw new Exception($"Unable to download blob {fileName}!");
            }

            var blobContent = blobDownloadResult.Value;

            return blobContent.Content.ToStream();
        }

        public async Task<bool> RemoveFile(string directory, string fileName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(directory);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            var blobDeleteResult = await blobClient.DeleteIfExistsAsync();

            if (blobDeleteResult.GetRawResponse().IsError)
            {
                throw new Exception($"Unable to delete blob {fileName}!");
            }

            return blobDeleteResult.Value;
        }

        public async Task UpdateFileInFileStorage(string directory, string fileName, IFormFile file)
        {
            await UploadFile(directory, fileName, file);
        }
    }
}
