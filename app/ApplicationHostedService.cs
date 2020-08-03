using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shared.Services;

namespace app
{
    public sealed class ApplicationHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ICosmosService _service;

        public ApplicationHostedService(ICosmosService service, ILogger<ApplicationHostedService> logger)
        {
            _service = service;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(StartAsync));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(StopAsync));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}