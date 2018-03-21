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
            return new Lookup[]
            {
                new Lookup() { Id = Convert.ToInt32(VisitorStatusEnum.Active), Name = "Active" },
                new Lookup() { Id = Convert.ToInt32(VisitorStatusEnum.Inactive), Name = "Inactive" },
                new Lookup() { Id = Convert.ToInt32(VisitorStatusEnum.Member), Name = "Member" }
            };
        }

        public static Lookup[] RoleLookup()
        {
            return new Lookup[]
            {
                new Lookup() { Id = Convert.ToInt32(UserRoleEnum.Admin), Name = "Admin" },
                new Lookup() { Id = Convert.ToInt32(UserRoleEnum.Editor), Name = "Editor" },
                new Lookup() { Id = Convert.ToInt32(UserRoleEnum.Viewer), Name = "Viewer" }
            };
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