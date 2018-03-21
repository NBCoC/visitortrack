using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class UserAuthenticated
    {
        public string Token { get; set; }

        public ReadonlyUser User { get; set; }
    }
}