using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class Visitor : IEntity
    {
        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public string FullName { get; set; }

        public string ContactNumber { get; set; }

        public DateTimeOffset? BecameMemberOn { get; set; }

        public DateTimeOffset? FirstVisitedOn { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public AgeGroupEnum AgeGroup { get; set; }

        public AgeGroupEnum[] KidsAgeGroups { get; set; } = new AgeGroupEnum[0];

        public CheckListItem[] CheckList { get; set; } = new CheckListItem[0];

        public Comment[] Comments { get; set; } = new Comment[0];

        public static Lookup[] AgeGroupLookup()
        {
            return new Lookup[]
            {
                new Lookup() { Id = Convert.ToInt32(AgeGroupEnum.Unknown), Name = "Unknow" },
                new Lookup() { Id = Convert.ToInt32(AgeGroupEnum.Group18to29), Name = "18 - 29" },
                new Lookup() { Id = Convert.ToInt32(AgeGroupEnum.Group29to39), Name = "29 - 39" },
                new Lookup() { Id = Convert.ToInt32(AgeGroupEnum.Group40to50), Name = "40 - 50" },
                new Lookup() { Id = Convert.ToInt32(AgeGroupEnum.Group50to59), Name = "50 - 50" },
                new Lookup() { Id = Convert.ToInt32(AgeGroupEnum.Group60plus), Name = "60+" }
            };
        }

        public static Lookup[] StatusLookup()
        {
            return new Lookup[]
            {
                new Lookup() { Id = Convert.ToInt32(VisitorStatusEnum.Active), Name = "Active" },
                new Lookup() { Id = Convert.ToInt32(VisitorStatusEnum.Inactive), Name = "Inactive" },
                new Lookup() { Id = Convert.ToInt32(VisitorStatusEnum.Member), Name = "Member" }
            };
        }
    }
}
