using Newtonsoft.Json;
using System;
using System.Globalization;

namespace VisitorTrack.Entities
{
     [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class VisitorReportItem 
    {
        public int MonthId { get; set; }

        public string Month 
            => (MonthId > 0 && MonthId < 13) ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(MonthId) : "Invalid month name";

        public VisitorLite[] Visitors { get; set; } = new VisitorLite[0];

        public VisitorLite[] Members { get; set; } = new VisitorLite[0];
    }
}