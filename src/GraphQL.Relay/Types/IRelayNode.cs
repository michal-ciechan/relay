using System.Threading.Tasks;

namespace GraphQL.Relay.Types
{
    public interface IRelayNode<T>
    {
        Task<T> GetById(string id, IResolveFieldContext<object> context);
    }
}