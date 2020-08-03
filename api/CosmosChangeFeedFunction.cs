using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Entities;
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

        private const string FunctionName = "ProcessChangeFeed";

        [FunctionName(FunctionName)]
        public async Task ProcessAsync(
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
            log.LogInformation("{FunctionName} - start - Items to be processed '{Count}'", FunctionName, input.Count);
            await ProcessDocumentsAsync(input);
            log.LogInformation("{FunctionName} - finished", FunctionName);
        }

        private async Task ProcessDocumentsAsync(IReadOnlyList<Document> documents)
        {
            var testByCategoryDocs = documents.Select(s => JsonConvert.DeserializeObject<TestByCategory>(s.ToString()));

            var entities = testByCategoryDocs
                .Select(s => new TestByDateTime
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category,
                    Month = s.DateTimeOffset.Month,
                    Year = s.DateTimeOffset.Year
                })
                .ToList();

            if (_service is IBulkExecutorCosmosService bulkExecutorService)
                await bulkExecutorService.BulkUpsertItemsAsync(entities);
            else
                throw new NotSupportedException("bulk execution is not supported");
        }
    }
}
