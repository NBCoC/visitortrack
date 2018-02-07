using Newtonsoft.Json;

namespace VisitorTrack.Entities.Models
{
    public class User 
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }  

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public UserRoleEnum RoleId { get; set; }

        public string DisplayName { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}