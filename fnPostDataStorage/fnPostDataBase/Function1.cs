using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace fnPostDataBase
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("filme")]
        [CosmosDBOutput("%DatabaseName%", "%CollectionName%", Connection = "CosmosDBConnection", CreateIfNotExists = true, PartitionKey = "id")]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            FilmeRequest filme = null;

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                filme = JsonConvert.DeserializeObject<FilmeRequest>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deserializar o objeto.");
                return new BadRequestObjectResult("Erro ao deserializar o objeto.");
            }


            return JsonConvert.SerializeObject(filme);
        }
    }
}
