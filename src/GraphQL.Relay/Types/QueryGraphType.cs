using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Types;

namespace GraphQL.Relay.Types
{
    public class QueryGraphType : ObjectGraphType
    {
        public QueryGraphType()
        {
            Name = "Query";

            Field<RelayNodeInterface>()
                .Name("node")
                .Description("Fetches an object given its global Id")
                .Argument<IdGraphType>("id", "The global Id of the object")
                .ResolveAsync(ResolveObjectFromGlobalId);
        }

        private Task<object> ResolveObjectFromGlobalId(IResolveFieldContext<object> context)
        {
            var globalId = context.GetArgument<string>("id");
            var parts = RelayNode.FromGlobalId(globalId);
            
            var type = parts.Type;
            var id = parts.Id;
            
            var node = (IRelayNode)context.Schema.FindType(type);

            return node.GetById(id, context);
        }
    }
}
