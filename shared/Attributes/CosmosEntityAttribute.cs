using System;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class CosmosEntityAttribute : Attribute
    {
        public string ContainerId { get; }
        public string PartitionKeyPath { get; }

        public CosmosEntityAttribute([NotNull] string containerId, [NotNull] string partitionKeyPath)
        {
            ContainerId = containerId;
            PartitionKeyPath = partitionKeyPath;
        }
    }
}
