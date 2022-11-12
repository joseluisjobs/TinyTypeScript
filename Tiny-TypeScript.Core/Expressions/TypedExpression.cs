using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Expressions
{
    public abstract class TypedExpression : Node
    {
        protected readonly Type type;

        public Token Token { get; }

        public TypedExpression(Type type, Token token)
        {
            Token = token;
            this.type = type;
        }

        public abstract Type GetExpressionType();

        public abstract string GenerateCode();

        public abstract dynamic Evaluate();
    }
}
