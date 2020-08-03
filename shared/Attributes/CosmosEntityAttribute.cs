using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true
    )]
    public sealed class CosmosEntityAttribute : Attribute
    {
        /// <summary>
        /// container that holds the entity
        /// </summary>
        public string ContainerId { get; }

        /// <summary>
        /// path to the field that contains the combined value of <see cref="PartitionKeyProperties"/>
        /// </summary>
        public string PartitionKeyPath { get; }

        /// <summary>
        /// properties that make up the partition key value
        /// </summary>
        public string[] PartitionKeyProperties { get; set; }

        /// <summary>
        /// when more than one value in <see cref="PartitionKeyProperties"/>, value to separate
        /// </summary>
        public char PartitionKeyPropertySeparator { get; }

        public CosmosEntityAttribute() { }

        public CosmosEntityAttribute(
            string containerId, 
            string[] partitionKeyProperties, 
            string partitionKeyPath = "/partitionKey",
            char partitionKeySeparator = '-')
        {
            ContainerId = containerId;
            PartitionKeyProperties = partitionKeyProperties;
            PartitionKeyPath = partitionKeyPath;
            PartitionKeyPropertySeparator = partitionKeySeparator;
        }
    }
}
