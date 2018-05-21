using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
     [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class VisitorLite : IEntity
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public bool IsMember => BecameMemberOn.HasValue;

        public bool IsActive { get; set; }

        public DateTimeOffset? BecameMemberOn { get; set; }

        public DateTimeOffset? FirstVisitedOn { get; set; }
    }
}