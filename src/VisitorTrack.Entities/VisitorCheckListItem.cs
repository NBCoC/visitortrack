using System;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class VisitorCheckListItem : CheckListItem
    {
        public DateTimeOffset? CompletedOn { get; set; }

        public string CompletedBy { get; set; }

        public string Comment { get; set; }
    }
}