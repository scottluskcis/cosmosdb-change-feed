using System;
using Newtonsoft.Json;

namespace Shared.Entities
{
    public abstract class BaseEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("docType")]
        public string Type => this.GetType().Name;
    }
}
