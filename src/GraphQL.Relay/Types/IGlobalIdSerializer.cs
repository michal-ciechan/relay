namespace GraphQL.Relay.Types
{
    public interface IGlobalIdSerializer
    {
        string ToGlobalId(string typeName, object id);

        GlobalId FromGlobalId(string globalId);
    }
}