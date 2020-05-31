using GraphQL.Types;

namespace GraphQL.Relay.Types
{
    public class RelayNodeInterface : InterfaceGraphType
    {
        public RelayNodeInterface()
        {
            Name = "RelayNode";

            Field<NonNullGraphType<IdGraphType>>("id", "Global node Id");
        }
    }
}