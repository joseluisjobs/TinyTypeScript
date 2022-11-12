using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Expressions
{
    public class ArrayAccessExpression : TypedExpression
    {
        public IdExpression Id { get; }

        public TypedExpression Index { get; }

        public ArrayAccessExpression(Type type, Token token, IdExpression id, TypedExpression index)
            : base(type, token)
        {
            Id = id;
            Index = index;
        }

        public override Type GetExpressionType()
        {
            return type;
        }

        public override string GenerateCode()
        {
            return $"{this.Id.GenerateCode()}[{this.Index.GenerateCode()}]";
        }

        public override dynamic Evaluate()
        {
            var symbol = this.Id.Evaluate();
            var position = this.Index.Evaluate();
            return symbol[(int)position];
        }
    }
}
