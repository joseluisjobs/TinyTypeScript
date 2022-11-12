using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Expressions
{
    public abstract class BinaryExpression : TypedExpression
    {
        public TypedExpression LeftExpression { get; }

        public TypedExpression RightExpression { get; }

        public BinaryExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression, Type type)
            :base(type, token)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }
    }
}
