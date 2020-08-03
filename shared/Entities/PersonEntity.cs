using System;
using Shared.Attributes;
using Shared.Constants;

namespace Shared.Entities
{
    [CosmosEntity(
        Containers.PersonContainerId, 
        "/" + nameof(LastName)
    )]
    public class PersonEntity : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
