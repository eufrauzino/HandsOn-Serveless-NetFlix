using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace fnPostDataStorage
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("dataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processando a Imagem no Log.");
            try
            {
                if(!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
                {
                    return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório.");
                }
                var fileType = fileTypeHeader.ToString();
                var form = req.ReadFormAsync();
                var file = req.Form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("O arquivo não foi enviado.");
                }
                    
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string containerName = fileType;
                BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);
                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

                await containerClient.CreateIfNotExistsAsync();

                await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

                string blobName = file.FileName;
                var blob = containerClient.GetBlobClient(blobName);
                using (var stream = file.OpenReadStream())
                {
                    await blob.UploadAsync(stream, true);
                }

                _logger.LogInformation($"Arquivo {file.FileName} salvo com sucesso.");


                return new OkObjectResult(new
                {
                    message = "Arquivo salvo com sucesso.",
                    BlobUri = blob.Uri
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a imagem.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }            
        }
    }
}
