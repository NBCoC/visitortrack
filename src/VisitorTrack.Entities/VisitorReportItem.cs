using Newtonsoft.Json;
using System;

namespace VisitorTrack.Entities
{
     [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class VisitorReportItem 
    {
        public int MemberCount { get; set; }

        public int VisitorCount { get; set; }

        public int MonthId { get; set; }

        public string Month { get; set; }
    }
}