using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class Visitor : VisitorSearch
    {
        public string Description { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public AgeGroupEnum[] KidsAgeGroups { get; set; } = new AgeGroupEnum[0];

        public VisitorCheckListItem[] CheckList { get; set; } = new VisitorCheckListItem[0];
    }
}
