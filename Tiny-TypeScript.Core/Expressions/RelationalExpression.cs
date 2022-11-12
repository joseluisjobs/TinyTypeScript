using System.Collections.Generic;
using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Expressions
{
    public class RelationalExpression : BinaryExpression
    {
        private readonly Dictionary<(Type, Type), Type> _typeRules;

        public RelationalExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
            : base(token, leftExpression, rightExpression, null)
        {
            _typeRules = new Dictionary<(Type, Type), Type>
            {
                { (Type.Number, Type.Number),  Type.Bool},
                { (Type.String, Type.String),  Type.Bool},
            };
        }

        public override dynamic Evaluate()
        {
            switch(Token.TokenType)
            {
                case TokenType.GreaterOrEqualThan: return this.LeftExpression.Evaluate() >= this.RightExpression.Evaluate();
                case TokenType.GreaterThan: return this.LeftExpression.Evaluate() > this.RightExpression.Evaluate();
                case TokenType.LessOrEqualThan: return this.LeftExpression.Evaluate() <= this.RightExpression.Evaluate();
                case TokenType.LessThan: return this.LeftExpression.Evaluate() < this.RightExpression.Evaluate();
                default: throw new System.NotImplementedException();
            };
        }

        public override string GenerateCode()
        {
            var leftCode = this.LeftExpression.GenerateCode();
            var rightCode = this.RightExpression.GenerateCode();
            return $"{leftCode} {this.Token.Lexeme} {rightCode}";
        }

        public override Type GetExpressionType()
        {
            var leftType = LeftExpression.GetExpressionType();
            var rightType = RightExpression.GetExpressionType();
            if (_typeRules.TryGetValue((leftType, rightType), out var resultType))
            {
                return resultType;
            }

            throw new System.ApplicationException($"Cannot perform relational operation on types {leftType} and {rightType}");
        }
    }
}
