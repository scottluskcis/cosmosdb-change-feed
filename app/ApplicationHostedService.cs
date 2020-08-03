using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using App;
using Microsoft.Extensions.Hosting;

namespace app
{
    public sealed class ApplicationHostedService : IHostedService, IDisposable
    {
        private readonly IApplicationRunner _service;
        private readonly ILogger _logger;

        public ApplicationHostedService(IApplicationRunner service, ILogger<ApplicationHostedService> logger)
        {
            _service = service;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(StartAsync));

            return _service.RunAsync();
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