using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class Visitor : VisitorSearch
    {
        public string Description { get; set; }

        public string EmailAddress { get; set; }

        public string ContactNumber { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public VisitorCheckListItem[] CheckList { get; set; } = new VisitorCheckListItem[0];
    }
}
