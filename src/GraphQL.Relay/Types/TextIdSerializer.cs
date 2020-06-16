using System.Linq;

namespace GraphQL.Relay.Types
{
    public class TextIdSerializer : IGlobalIdSerializer
    {
        public string ToGlobalId(string typeName, object id)
        {
            return "{0}/{1}".ToFormat(typeName, id);
        }

        public GlobalId FromGlobalId(string globalId)
        {
            var parts = globalId.Split('/');
            
            return new GlobalId {
                Type = parts[0],
                Id = parts[1],
            };
        }
    }
}