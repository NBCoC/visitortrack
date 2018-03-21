using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class Lookup
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}