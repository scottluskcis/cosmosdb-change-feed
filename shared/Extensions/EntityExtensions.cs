using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Shared.Attributes;
using Shared.Entities;

namespace Shared.Extensions
{
    public static class EntityExtensions
    {
        public static ContainerProperties GetContainerProperties(this Type type)
        {
            var attribute = type
                .GetCustomAttributes(typeof(CosmosEntityAttribute), true)
                .OfType<CosmosEntityAttribute>()
                .Single();

            var properties = new ContainerProperties(
                attribute.ContainerId, 
                attribute.PartitionKeyPath);

            return properties;
        }

        public static PartitionKey GetPartitionKey(this BaseEntity entity)
        {
            var type = entity.GetType();
            var props = type.GetContainerProperties();

            if (string.IsNullOrEmpty(props?.PartitionKeyPath))
            {
                return PartitionKey.None;
            }
            else
            {
                var value = type
                    .GetProperty(props.PartitionKeyPath)
                    ?.GetValue(entity)
                    ?.ToString();

                var partitionKey = value != null ? new PartitionKey(value) : PartitionKey.Null;
                return partitionKey;
            }
        }
    }
}
