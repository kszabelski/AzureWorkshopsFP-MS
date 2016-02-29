using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Common
{
    public class FilesStorageService
    {
        private const string ContainerName = "constructionprogressfiles";
        private readonly CloudBlobContainer _container;
        private readonly StorageUri _blobStorageUri;

        public FilesStorageService(string storageConnectionString)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString));
            }

            CloudStorageAccount storageAccount;
            if (!CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                throw new ArgumentException("Could not parse the connection string.", nameof(storageConnectionString));
            }

            _blobStorageUri = storageAccount.BlobStorageUri;
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(ContainerName);
        }

        public async Task<string> UploadFile(string fileName, Stream file, string mimeType)
        {
            if (!_container.Exists())
            {
                _container.Create();
                _container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = mimeType;
            await blockBlob.UploadFromStreamAsync(file);

            return blockBlob.Uri.AbsoluteUri;
        }
    }
}