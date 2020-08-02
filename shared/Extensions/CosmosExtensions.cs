using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Shared.Extensions
{
    public static class CosmosExtensions
    {
        public static IServiceCollection AddCosmosClient(this IServiceCollection services)
        {
            return services.AddSingleton((provider) =>
            {
                var options = provider.GetRequiredService<IOptions<CosmosDbConfiguration>>();
                var cosmosDbConfig = options?.Value;
                return cosmosDbConfig.CreateCosmosClient();
            });
        }

        internal static CosmosClient CreateCosmosClient([NotNull] this CosmosDbConfiguration configuration)
        {
            var clientBuilder = new CosmosClientBuilder(
                configuration.EndpointUri,
                configuration.PrimaryKey);
                        
            if (configuration.UseThrottling) 
                clientBuilder = clientBuilder
                    .WithThrottlingRetryOptions(
                        configuration.MaxRetryWaitTimeOnThrottledRequests ?? new TimeSpan(0, 0, 0, 30),
                        configuration.MaxRetryAttemptsOnThrottledRequests ?? 3);

            if (configuration.ConsistencyLevel.HasValue)
                clientBuilder = clientBuilder
                    .WithConsistencyLevel(configuration.ConsistencyLevel.Value);

            if (configuration.AllowBulkExecution.HasValue)
                clientBuilder = clientBuilder
                    .WithBulkExecution(configuration.AllowBulkExecution.Value);

            if(configuration.CamelCasePropertyNames)
                clientBuilder = clientBuilder.WithSerializerOptions(new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                });

            return clientBuilder.Build();
        }
    }
}
