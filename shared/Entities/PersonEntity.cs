using System;
using Shared.Attributes;

namespace Shared.Entities
{
    [CosmosEntity("Person", "/" + nameof(LastName))]
    public class PersonEntity : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
