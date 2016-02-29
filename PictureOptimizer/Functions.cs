using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Common;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;

namespace PictureOptimizer
{
    public class Functions
    {
        public static void ProcessQueueMessage([ServiceBusTrigger("resizepicturesqueue")] ResizePictureMessage message, TextWriter logger)
        {
            var azureStorageConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;

            byte[] data;
            using (var client = new WebClient())
            {
                data = client.DownloadData(message.PictureReference);
            }

            var thumbnail = ImageResizer.CreateThumbnail(Image.FromStream(new MemoryStream(data)));

            var filesStorageService = new FilesStorageService(azureStorageConnectionString);

            var uploadedFileUrl = filesStorageService.UploadFile(Guid.NewGuid().ToString(), thumbnail, "image/jpeg").Result;

            using (var context = new ConstructionsProgressTrackerContext())
            {
                context.ProgressTrackingEntries.Single(e => e.Id == message.Id).ThumbnailPictureReference = uploadedFileUrl;
                context.SaveChanges();
            }

            logger.WriteLine($"Thumbnail for entry: {message.Id} successfully created. Available at {uploadedFileUrl}.");
        }
    }
}
