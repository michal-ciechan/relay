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
            var node = (IRelayNode<object>) context.Schema.FindType(parts.Type);

            return node.GetById(parts.Id, context);
        }
    }
}
