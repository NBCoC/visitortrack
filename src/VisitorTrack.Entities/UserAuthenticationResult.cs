using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class UserAuthenticationResult
    {
        public string Token { get; set; }

        public User User { get; set; }
    }
}