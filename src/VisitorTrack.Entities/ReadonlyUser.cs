using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class ReadonlyUser : IEntity
    {
        public string Id { get; set; }  

        public string EmailAddress { get; set; }

        public UserRoleEnum RoleId { get; set; }

        public string DisplayName { get; set; }
    }
}