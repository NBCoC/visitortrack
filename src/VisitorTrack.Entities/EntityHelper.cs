using System;
using System.Linq;

namespace VisitorTrack.Entities
{
    public class EntityHelper
    {
        public static Lookup[] AgeGroupLookup()
        {
            var ageGroups = new AgeGroupEnum[]
            {
                AgeGroupEnum.Unknown,
                AgeGroupEnum.Group18to29,
                AgeGroupEnum.Group29to39,
                AgeGroupEnum.Group40to50,
                AgeGroupEnum.Group50to59,
                AgeGroupEnum.Group60plus
            };

            return
                ageGroups
                    .Select(ageGroup => new Lookup() { Id = Convert.ToInt32(ageGroup), Name = GetAgeGroupName(ageGroup) })
                    .ToArray();
        }

        public static Lookup[] StatusLookup()
        {
            var statusList = new VisitorStatusEnum[]
            {
                VisitorStatusEnum.Active,
                VisitorStatusEnum.Inactive,
                VisitorStatusEnum.Member
            };

            return 
                statusList
                    .Select(status => new Lookup() { Id = Convert.ToInt32(status), Name = status.ToString() })
                    .ToArray();
        }

        public static Lookup[] RoleLookup()
        {
            var roles = new UserRoleEnum[]
            {
                UserRoleEnum.Admin,
                UserRoleEnum.Editor,
                UserRoleEnum.Viewer
            };

            return 
                roles
                    .Select(role => new Lookup() { Id = Convert.ToInt32(role), Name = role.ToString() })
                    .ToArray();
        }

        public static string GetAgeGroupName(AgeGroupEnum ageGroup)
        {
            switch (ageGroup)
            {
                case AgeGroupEnum.Group18to29:
                    return "18 - 19";

                case AgeGroupEnum.Group29to39:
                    return "29 - 39";

                case AgeGroupEnum.Group40to50:
                    return "40 - 50";

                case AgeGroupEnum.Group50to59:
                    return "50 - 59";

                case AgeGroupEnum.Group60plus:
                    return "60+";

                default:
                    return "Unknown";
            }
        }
    }
}