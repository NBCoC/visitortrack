using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VisitorTrack.Entities
{
    [JsonObject(NamingStrategyType = typeof(LowercaseNamingStrategy))]
    public class User : ReadonlyUser
    {
        public string Password { get; set; }

        public static Lookup[] RoleLookup()
        {
            return new Lookup[]
            {
                new Lookup() { Id = Convert.ToInt32(UserRoleEnum.Admin), Name = "Admin" },
                new Lookup() { Id = Convert.ToInt32(UserRoleEnum.Editor), Name = "Editor" },
                new Lookup() { Id = Convert.ToInt32(UserRoleEnum.Viewer), Name = "Viewer" }
            };
        }
    }
}