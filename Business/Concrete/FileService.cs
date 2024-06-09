using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Business.Abstract;
using Core.CrossCuttingConcerns;
using Core.Utils.Azure;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class FileService : IFileService
{
    private readonly BlobServiceClient _blobServiceClient; 
    private readonly AzureSettings _azureSettings;
    public FileService(BlobServiceClient blobServiceClient, AzureSettings azureSettings)
    {
        _blobServiceClient = blobServiceClient;
        _azureSettings = azureSettings;
    }


    public List<BlobContainerItem> GetContainerList()
    {
        var listOfContainers = _blobServiceClient.GetBlobContainersAsync();
        var list = listOfContainers.ToBlockingEnumerable().ToList();
        return list;
    }

    public List<BlobItem> GetBlobList(string containerName)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        var list = blobContainer.GetBlobsAsync().ToBlockingEnumerable().ToList();
        return list;
    }

    public async Task<string> UploadAsync(IFormFile file, string containerName, string? blobDir, string? customFileName)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

        BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainer.CreateIfNotExistsAsync();

        if (!string.IsNullOrEmpty(customFileName)) // add file ext type to custom name if not exist
        {
            string fileExtension = Path.GetExtension(file.FileName);
            if (!customFileName.EndsWith(fileExtension)) customFileName += fileExtension;
        }

        string blobName = string.IsNullOrEmpty(blobDir) == false ? $"{blobDir}/{customFileName ?? file.FileName}" : customFileName ?? file.FileName;
        BlobClient blob = blobContainer.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            if (stream == null) throw new Exception("File Stream Could not read");
            await blob.UploadAsync(content: stream, overwrite: true);
        };

        // Blob URL'sini CDN URL'sine dönüştür
        string cdnDomainWithContainer = $"{_azureSettings.CdnDomain}/{containerName}";
        string cdnUrl = blob.Uri.ToString().Replace(blobContainer.Uri.ToString(), cdnDomainWithContainer); 
        return cdnUrl;
    }

    public async Task<bool> DeleteAsync(string containerName, string blobName)
    {
        if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
        if (string.IsNullOrWhiteSpace(blobName)) throw new ArgumentNullException(nameof(blobName));

        BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        bool isContainerExist = await blobContainer.ExistsAsync();
        if (isContainerExist == false) throw new Exception($"Not exist any container for {containerName}");
        BlobClient blob = blobContainer.GetBlobClient(blobName); 
        bool isBlobExist = await blob.ExistsAsync();
        if (isBlobExist == false) throw new Exception($"Not exist blob for {blobName}");
        bool isDeleted = await blob.DeleteIfExistsAsync();
        return isDeleted;
    }
}