using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Types.Relay;
using Panic.StringUtils;

namespace GraphQL.Relay.Types
{
    public abstract class NodeGraphType<T, TOut> : ObjectGraphType<T>, IRelayNode<TOut>
    {
        public static Type Edge => typeof(EdgeType<NodeGraphType<T, TOut>>);

        public static Type Connection => typeof(ConnectionType<NodeGraphType<T, TOut>>);

        protected NodeGraphType()
        {
            Interface<RelayNodeInterface>();
        }

        public abstract Task<TOut> GetById(string id, IResolveFieldContext<object> context);

        public FieldType Id<TReturnType>(Expression<Func<T, TReturnType>> expression)
        {
            string name = null;
            try
            {
                name = StringUtils.ToCamelCase(expression.NameOf());
            }
            catch
            {
            }


            return Id(name, expression);
        }

        public FieldType Id<TReturnType>(string name, Expression<Func<T, TReturnType>> expression)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // if there is a field called "ID" on the object, namespace it to "contactId"
                if (name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        throw new InvalidOperationException(
                            "The parent GraphQL type must define a Name before declaring the Id field " +
                            "in order to properly prefix the local id field");

                    name = StringUtils.ToCamelCase(Name + "Id");
                }

                Field(name, expression)
                    .Description($"The Id of the {Name ?? "node"}")
                    .FieldType.Metadata["RelayLocalIdField"] = true;
            }

            var compiledExpression = expression.Compile();
            
            var field = Field(
                name: "id",
                description: $"The Global Id of the {Name ?? "node"}",
                type: typeof(NonNullGraphType<IdGraphType>),
                resolve: context => RelayNode.ToGlobalId(
                    context.ParentType.Name,
                    compiledExpression(context.Source)
                )
            );

            field.Metadata["RelayGlobalIdField"] = true;

            if (!string.IsNullOrWhiteSpace(name))
                field.Metadata["RelayRelatedLocalIdField"] = name;

            return field;
        }

        async Task<object> IRelayNode.GetById(string id, IResolveFieldContext<object> context) => await GetById(id, context);
    }

    public abstract class NodeGraphType<TSource> : NodeGraphType<TSource, TSource>
    {
    }

    public abstract class NodeGraphType : NodeGraphType<object>
    {
    }
}
