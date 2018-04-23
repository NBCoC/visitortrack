using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
     [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class VisitorSearch : VisitorLite
    {
        public AgeGroupEnum AgeGroupId { get; set; }

        public string AgeGroupName => AgeGroupId.GetName();
    }
}