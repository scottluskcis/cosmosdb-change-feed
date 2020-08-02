using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
                var value = entity.GetPartitionKeyValue();
                var partitionKey = value != null ? new PartitionKey(value) : PartitionKey.Null;
                return partitionKey;
            } 
        }
        
        public static string GetPartitionKeyValue(this BaseEntity entity)
        {
            var type = entity.GetType();
            var props = type.GetContainerProperties();

            var propertyName =
                props.PartitionKeyPath.StartsWith("/")
                    ? props.PartitionKeyPath.Substring(1)
                    : props.PartitionKeyPath;

            var value = type
                .GetProperties()
                .SingleOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                ?.GetValue(entity)
                ?.ToString();

            return value;
        }
    }
}
