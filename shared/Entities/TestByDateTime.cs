using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Attributes;

namespace Shared.Entities
{
    [CosmosEntity(
        "feed",
        new [] { nameof(Month), nameof(Year) }
    )]
    public class TestByDateTime : BaseEntity
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public override string GetId() => Id.ToString();
    }
}
