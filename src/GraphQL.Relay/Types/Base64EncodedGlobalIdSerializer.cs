using System.Linq;
using Panic.StringUtils;

namespace GraphQL.Relay.Types
{
    public class Base64EncodedGlobalIdSerializer : IGlobalIdSerializer
    {
        public string ToGlobalId(string typeName, object id)
        {
            var decodedGlobalId = "{0}/{1}".ToFormat(typeName, id);

            var result = StringUtils.Base64Encode(decodedGlobalId);

            return result;
        }

        public GlobalId FromGlobalId(string globalId)
        {
            var decodedGlobalId = StringUtils.Base64Decode(globalId);
            
            var parts = decodedGlobalId.Split(':');
            
            return new GlobalId {
                Type = parts[0],
                Id = string.Join("/", parts.Skip(count: 1)),
            };
        }
    }
}