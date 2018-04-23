using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class UserAuthentication
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }
    }
}