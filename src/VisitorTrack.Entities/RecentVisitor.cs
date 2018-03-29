using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
     [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class RecentVisitor : IEntity
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public VisitorStatusEnum StatusId { get; set; }

        public string StatusName => StatusId.ToString();
    }
}