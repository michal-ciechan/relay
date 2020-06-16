using System;
using System.Threading.Tasks;

namespace GraphQL.Relay.Types
{
    public static class RelayNode
    {
        public static NodeGraphType<TSource, TOut> For<TSource, TOut>(Func<string, IResolveFieldContext<object>, Task<TOut>> getById)
        {
            var type = new DefaultNodeGraphType<TSource, TOut>(getById);

            return type;
        }

        public static IGlobalIdSerializer GlobalIdSerializer { get; set; } = new Base64EncodedGlobalIdSerializer();
        public static ICursorSerializer CursorSerializer { get; set; } = new TextCursorSerializer();

        public static string ToGlobalId(string name, object id)
        {
            return GlobalIdSerializer.ToGlobalId(name, id);
        }

        public static GlobalId FromGlobalId(string globalId)
        {
            return GlobalIdSerializer.FromGlobalId(globalId);
        }
    }
}