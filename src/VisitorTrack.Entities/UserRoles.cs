using System;
using System.Collections.Generic;
using System.Linq;

namespace VisitorTrack.Entities
{
    public enum UserRoleEnum
    {
        Viewer = 0,
        Admin = 1,
        Editor = 2
    }

    public static class UserRoles
    {
        public static string GetName(this UserRoleEnum role) => role.ToString();

        public static KeyValuePair<int, string>[] AsLookup()
        {
            var roles = new UserRoleEnum[]
            {
                UserRoleEnum.Admin,
                UserRoleEnum.Editor,
                UserRoleEnum.Viewer
            };

            return 
                roles
                    .Select(role => new KeyValuePair<int, string>(Convert.ToInt32(role), role.ToString()))
                    .ToArray();
        }
    }
}