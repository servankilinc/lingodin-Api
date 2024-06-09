using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract;

public interface IFileService
{
    List<BlobContainerItem> GetContainerList();
    List<BlobItem> GetBlobList(string containerName);
    Task<string> UploadAsync(IFormFile file, string containerName, string? blobDir, string? customFileName);
    Task<bool> DeleteAsync(string containerName, string blobFilename);
}