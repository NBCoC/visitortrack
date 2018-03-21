using System;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class Comment : IEntity
    {
        public string Id { get; set; }  

        public DateTimeOffset Date { get; set; }

        public string Text { get; set; }

        public string Commentor { get; set; }
    }
}