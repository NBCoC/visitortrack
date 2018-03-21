using System;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class CheckListItem : IEntity
    {
        public string Id { get; set; }  

        public string Description { get; set; }

        public string Type { get; set; }

        public DateTimeOffset? CompletedOn { get; set; }

        public string CompletedBy { get; set; }
    }
}