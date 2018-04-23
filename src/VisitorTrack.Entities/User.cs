using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class User : IEntity
    {
        public string Id { get; set; }  

        public string EmailAddress { get; set; }

        public UserRoleEnum RoleId { get; set; }

        public string RoleName => RoleId.GetName();

        public string DisplayName { get; set; }
    }
}