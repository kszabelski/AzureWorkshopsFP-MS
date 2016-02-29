using System.IO;
using System.Threading.Tasks;

namespace Common
{
    public class FilesStorageService
    {
        public Task<string> UploadFile(string fileName, Stream file, string mimeType)
        {
            ////var storageAccount = CloudStorageAccount.Parse("azureStorageConnectionString");
            ////var blobClient = storageAccount.CreateCloudBlobClient();
            ////var container = blobClient.GetContainerReference("containername");
            ////container.CreateIfNotExists();
            ////container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
            ////var blockBlob = container.GetBlockBlobReference(fileName);
            ////blockBlob.Properties.ContentType = mimeType;
            ////await blockBlob.UploadFromStreamAsync(file);

            return Task.FromResult(string.Empty);
        }
    }
}