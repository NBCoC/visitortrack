using System;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    public class User : IEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Email { get; set; }

        public RoleEnum Role { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public string DisplayName { get; set; }

        public override string ToString() => EntityHelper.Serialize(this);
    }
}
