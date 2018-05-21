using System;
using System.Collections.Generic;
using System.Linq;

namespace VisitorTrack.Entities
{
    public enum AgeGroupEnum
    {
        Unknown = 0,
        Group18to29 = 1,
        Group29to39 = 2,
        Group40to50 = 3,
        Group50to59 = 4,
        Group60plus = 5
    }

    public static class AgeGroups
    {
        public static string GetName(this AgeGroupEnum ageGroup)
        {
            switch (ageGroup)
            {
                case AgeGroupEnum.Group18to29:
                    return "18 - 29";

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

        public static KeyValuePair<int, string>[] AsLookup()
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
                    .Select(ageGroup => new KeyValuePair<int, string>(Convert.ToInt32(ageGroup), ageGroup.GetName()))
                    .ToArray();
        }
    }
}