using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VisitorTrack.Entities
{
    public static class EntityHelper
    {
        public static string Serialize(object entity)
            => JsonConvert.SerializeObject(entity,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
    }
}