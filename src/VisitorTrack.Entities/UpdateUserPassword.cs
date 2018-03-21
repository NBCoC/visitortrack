using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class UpdateUserPassword
    {
        public string NewPassword { get; set; }

        public string OldPassword { get; set; }
    }
}