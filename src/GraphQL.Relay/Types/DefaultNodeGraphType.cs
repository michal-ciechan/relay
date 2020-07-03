using System;
using System.Threading.Tasks;

namespace GraphQL.Relay.Types
{
    public class DefaultNodeGraphType<TSource, TOut> : NodeGraphType<TSource, TOut>
    {
        private readonly Func<string, IResolveFieldContext<object>, Task<TOut>> _getById;

        public DefaultNodeGraphType(Func<string, IResolveFieldContext<object>, Task<TOut>> getById)
        {
            _getById = getById;
        }

        public override Task<TOut> GetByRelayGlobalId(string id, IResolveFieldContext<object> context)
        {
            return _getById(id, context);
        }
    }
}