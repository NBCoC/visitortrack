using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class Visitor : VisitorSearch
    {
        public string ContactNumber { get; set; }

        public string EmailAddress { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? BecameMemberOn { get; set; }

        public DateTimeOffset? FirstVisitedOn { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public AgeGroupEnum[] KidsAgeGroups { get; set; } = new AgeGroupEnum[0];

        public CheckListItem[] CheckList { get; set; } = new CheckListItem[0];

        public Comment[] Comments { get; set; } = new Comment[0];
    }
}
