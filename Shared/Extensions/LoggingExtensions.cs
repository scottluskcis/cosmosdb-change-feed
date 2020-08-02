using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Shared.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogItemResponse<T>(this ILogger logger, [NotNull] ItemResponse<T> response)
        {
            logger.LogInformation("ItemResponse - ActivityId: {ActivityId}, StatusCode: {StatusCode}, RequestCharge: {RequestCharge}",
                response.ActivityId,
                response.StatusCode,
                response.RequestCharge);
        }

        public static void LogContainerResponse(this ILogger logger, [NotNull] ContainerResponse response)
        {
            logger.LogInformation("ContainerResponse - ContainerId: {ContainerId}, ActivityId: {ActivityId}, StatusCode: {StatusCode}, RequestCharge: {RequestCharge}",
                response.Container.Id,
                response.ActivityId,
                response.StatusCode,
                response.RequestCharge);
        }

        public static void LogDatabaseResponse(this ILogger logger, [NotNull] DatabaseResponse response)
        {
            logger.LogInformation("ContainerResponse - DatabaseId: {DatabaseId}, ActivityId: {ActivityId}, StatusCode: {StatusCode}, RequestCharge: {RequestCharge}",
                response.Database.Id,
                response.ActivityId,
                response.StatusCode,
                response.RequestCharge);
        }

        public static void LogPartitionKey(this ILogger logger, PartitionKey key, Container container)
        {
            logger.LogInformation("PartitionKey: {PartitionKey}, ContainerId: {ContainerId}", 
                key.ToString(),
                container.Id);
        }
    }
}
