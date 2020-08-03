using System;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Dates;
using Microsoft.Extensions.Logging;
using Shared.Entities;
using Shared.Services;

namespace App
{
    public interface IApplicationRunner
    {
        Task RunAsync();
    }

    internal class ApplicationRunner : IApplicationRunner
    {
        private readonly ILogger _logger;
        private readonly ICosmosService _service;

        public ApplicationRunner(ICosmosService service, ILogger<ApplicationRunner> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation($"{nameof(ApplicationRunner)} is running...");

            await SetupGroupEntityAsync();

            _logger.LogInformation($"{nameof(ApplicationRunner)} is finished");
        }

        private async Task SetupGroupEntityAsync()
        {
            await _service.CreateContainerIfNotExistsAsync<TestByCategory>();

            var generator = new RandomGenerator();

            var groups = Builder<TestByCategory>
                .CreateListOfSize(100)
                .All()
                    .With(x => x.Id = Guid.NewGuid())
                    .With(x => x.DateTimeOffset = generator.Next(January.The1st, December.The31st))
                    .With(x => x.Description = generator.NextString(25, 250))
                .TheFirst(25)
                    .With(x => x.Category = "Lorem")
                .TheNext(25)
                    .With(x => x.Category = "Ipsum")
                .TheNext(25)
                    .With(x => x.Category = "Sit")
                .TheNext(25)
                    .With(x => x.Category = "Dolor")
                .Build();

            if(_service is IBulkExecutorCosmosService bulkExecutorService)
                await bulkExecutorService.BulkCreateItemsAsync(groups);
            else
                throw new NotSupportedException("service does not support Bulk Execution");
        }

    }
}
