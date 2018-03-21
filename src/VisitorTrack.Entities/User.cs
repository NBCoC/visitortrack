using System;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class User : ReadonlyUser
    {
        public string Password { get; set; }
    }
}