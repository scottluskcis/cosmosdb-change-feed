using System;
using Newtonsoft.Json;
using Shared.Attributes;

namespace Shared.Entities
{
    [CosmosEntity(
        "data",
        new [] { nameof(Category) }
    )]
    public class TestByCategory : BaseEntity
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public override string GetId() => Id.ToString();
    }
}
