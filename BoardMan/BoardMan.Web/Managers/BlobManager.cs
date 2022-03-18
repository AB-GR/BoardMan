using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BoardMan.Web.Managers
{
    public interface IBlobManager
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);

        Task<(Stream, string, string)> DownloadAsync(string fileName);

        Task<bool> DeleteAsync(string fileName);
    }

    public class BlobManager : IBlobManager
    {
        private readonly string _storageConnectionString;
        public BlobManager(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }

		public async Task<bool> DeleteAsync(string fileName)
		{
            var container = new BlobContainerClient(_storageConnectionString, "file-container");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blob = container.GetBlobClient(fileName);
            var response = await blob.DeleteAsync();
            return response.Status == 200 ? true : false;  
        }

		public async Task<(Stream, string, string)> DownloadAsync(string fileName)
		{
            BlobClient blob;
            await using (MemoryStream memoryStream = new MemoryStream())
            {
                var container = new BlobContainerClient(_storageConnectionString, "file-container");
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);

                blob = container.GetBlobClient(fileName);
                await blob.DownloadToAsync(memoryStream);
            }

            Stream blobStream = blob.OpenReadAsync().Result;
            return (blobStream, blob.GetProperties().Value.ContentType, blob.Name);
        }

		public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var container = new BlobContainerClient(_storageConnectionString, "file-container");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            var blob = container.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            return blob.Uri.ToString();
        }
    }
}
