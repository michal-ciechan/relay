using System.Threading.Tasks;

namespace GraphQL.Relay.Types
{
    public interface IRelayNode
    {
        Task<object> GetById(string id, IResolveFieldContext<object> context);
    }
    
    public interface IRelayNode<T> : IRelayNode
    {
        new Task<T> GetById(string id, IResolveFieldContext<object> context);
    }
}