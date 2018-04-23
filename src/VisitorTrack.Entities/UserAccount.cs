using System;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class UserAccount : User
    {
        public string Password { get; set; }
    }
}