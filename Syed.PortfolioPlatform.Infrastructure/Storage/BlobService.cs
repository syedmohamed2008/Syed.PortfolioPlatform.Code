using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Syed.PortfolioPlatform.Application.Interfaces;

public class BlobService : IBlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        _connectionString = configuration["BlobStorage:ConnectionString"];
        _containerName = configuration["BlobStorage:ContainerName"];

        _containerClient = new BlobContainerClient(_connectionString, _containerName);
        _containerClient.CreateIfNotExists(); // run once
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();

        await blobClient.UploadAsync(stream, overwrite: true);

        return fileName;

    }
}