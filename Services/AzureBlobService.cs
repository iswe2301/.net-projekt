using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

// Klass för att ladda upp och radera filer i Azure Blob Storage
public class AzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobService(IConfiguration configuration)
    {
        // Hämta connection string från appsettings.json
        string? connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? throw new ArgumentNullException(nameof(_containerName), "Container name is not configured.");

        // Kontrollera att connection string inte är null
        if (string.IsNullOrEmpty(connectionString))
        {
            // Kasta ett undantag om connection string saknas
            throw new ArgumentNullException(nameof(connectionString), "Azure Blob Storage anslutningssträng saknas i appsettings.json.");
        }

        // Skapa en referens till Blob Storage-klienten
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // Metod för att ladda upp en fil
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        // Skapa en referens till containern
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Skapa containern om den inte redan finns
        await containerClient.CreateIfNotExistsAsync();

        // Skapa en referens till bloben
        var blobClient = containerClient.GetBlobClient(fileName);

        // Ladda upp filen och skriv över om den redan finns
        await blobClient.UploadAsync(fileStream, overwrite: true);

        // Returnera URL:en till den uppladdade filen
        return blobClient.Uri.ToString();
    }

    // Metod för att radera en fil
    public async Task DeleteFileAsync(string fileUrl)
    {
        // Skapa en referens till containern
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Hämta filnamnet från URL:en
        Uri uri = new Uri(fileUrl);
        string fileName = Path.GetFileName(uri.LocalPath);

        // Skapa en referens till bloben och radera den
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}
