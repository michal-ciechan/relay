using System.Threading.Tasks;

namespace GraphQL.Relay.Types
{
    public abstract class AsyncNodeGraphType<T> : NodeGraphType<T, Task<T>>
    {
    }
}