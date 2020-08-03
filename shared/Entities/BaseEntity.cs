using System.Dynamic;
using Newtonsoft.Json;
using Shared.Extensions;

namespace Shared.Entities
{
    public abstract class BaseEntity
    {
        public abstract string GetId();

        [JsonProperty("docType")]
        public string Type => this.GetType().Name;

        [JsonProperty("partitionKey")]
        public string PartitionKey => this.GetPartitionKeyValue();
    }
}
