using System;
using Microsoft.Azure.Cosmos;

namespace Shared.Configuration
{
    public class CosmosDbConfiguration
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public bool CreateIfNotExists { get; set; }
        public ConsistencyLevel? ConsistencyLevel { get; set; }
        public bool? AllowBulkExecution { get; set; }

        public bool UseThrottling =>
            MaxRetryAttemptsOnThrottledRequests.HasValue ||
            MaxRetryWaitTimeOnThrottledRequests.HasValue;

        public TimeSpan? MaxRetryWaitTimeOnThrottledRequests { get; set; } 
        public int? MaxRetryAttemptsOnThrottledRequests { get; set; }

        public bool CamelCasePropertyNames { get; set; }
    }
}