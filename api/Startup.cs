using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Services;

[assembly: FunctionsStartup(typeof(api.Startup))]
namespace api
{
    internal sealed class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddConfiguration<CosmosDbConfiguration>("CosmosDb");
            builder.Services.AddCosmosClient();
            builder.Services.AddTransient<ICosmosService, CosmosService>();
        }
    }
}
