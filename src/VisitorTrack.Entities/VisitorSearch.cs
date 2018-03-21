using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
     [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class VisitorSearch : RecentVisitor
    {
        public string StatusName => StatusId.ToString();

        public AgeGroupEnum AgeGroupId { get; set; }

        public string AgeGroupName => EntityHelper.GetAgeGroupName(AgeGroupId);
    }
}