using Newtonsoft.Json.Serialization;

namespace VisitorTrack.Entities
{
    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name) 
        {
            var first = name[0].ToString().ToLower();
            var remaining = name.Substring(1, name.Length - 1);

            return first + remaining;
        }
    }
}