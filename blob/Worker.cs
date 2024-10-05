using Azure.Storage.Blobs;

namespace blob
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=firststorage12;AccountKey=AELUKOQMM9JN5k8rwcb7HeXDzqr9NJGm9Ped6J1r0VSYqa8kwsl39DnA0hOUmNn0OiyydlekpOBX+ASth+K3Aw==;EndpointSuffix=core.windows.net"; // Replace with your Azure Storage connection string
        private const string containerName = "test"; // Replace with your container name
        private const string blobName = "sample.txt"; // Name of the blob to read/write

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get the container client
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await blobContainerClient.CreateIfNotExistsAsync();

            // Read data from the blob
            await ReadBlobDataAsync(blobContainerClient, blobName);

            // Save data to the blob
            string dataToSave = "Hello, Azure Blob Storage!";
            await SaveBlobDataAsync(blobContainerClient, blobName, dataToSave);
        }
        private static async Task ReadBlobDataAsync(BlobContainerClient containerClient, string blobName)
        {


            var blobClient = containerClient.GetBlobClient(blobName);
            Console.WriteLine($"Reading blob: {blobName}");

            try
            {
                var downloadInfo = await blobClient.DownloadAsync();
                using (var reader = new StreamReader(downloadInfo.Value.Content))
                {
                    string content = await reader.ReadToEndAsync();
                    Console.WriteLine($"Blob content: {content}");
                }
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                Console.WriteLine($"Blob {blobName} does not exist.");
            }
        }

        private static async Task SaveBlobDataAsync(BlobContainerClient containerClient, string blobName, string data)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            Console.WriteLine($"Saving data to blob: {blobName}");

            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data)))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
                Console.WriteLine("Data saved successfully.");
            }
        }
    }
}
