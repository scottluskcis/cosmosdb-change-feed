using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Shared.Services;

namespace api
{
    public class CosmosChangeFeedFunction
    {
        private readonly ICosmosService _service;

        public CosmosChangeFeedFunction(ICosmosService service)
        {
            _service = service;
        }

        [FunctionName("CosmosDataFeed")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "%CosmosDb:DatabaseId%",
                collectionName: "%CosmosDb:DataContainerName%",
                ConnectionStringSetting = "CosmosDbLocal",
                LeaseCollectionName = "%CosmosDb:LeaseContainerName%", 
                CreateLeaseCollectionIfNotExists = true
            )]
            IReadOnlyList<Document> input, 
            ILogger log)
        {
            await Task.Delay(10);
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
