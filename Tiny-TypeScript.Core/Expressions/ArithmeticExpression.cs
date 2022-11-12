using System.Collections.Generic;
using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Expressions
{
    public class ArithmeticExpression : BinaryExpression
    {
        private readonly Dictionary<(Type, Type, TokenType), Type> _typeRules;

        public ArithmeticExpression(Token token, TypedExpression leftExpression, TypedExpression rightExpression)
            : base(token, leftExpression, rightExpression, null)
        {
            _typeRules = new Dictionary<(Type, Type, TokenType), Type>
            {
                { (Type.Number, Type.Number, TokenType.Plus), Type.Number },
                { (Type.Number, Type.String, TokenType.Plus), Type.String },
                { (Type.String, Type.Number, TokenType.Plus), Type.String },
                { (Type.String, Type.String, TokenType.Plus), Type.String },
                { (Type.Number, Type.Number, TokenType.Minus), Type.Number },
                { (Type.Number, Type.Number, TokenType.Multiplication), Type.Number },
                { (Type.Number, Type.Number, TokenType.Division), Type.Number },
                { (Type.Bool, Type.Bool, TokenType.Plus), Type.Bool },
            };
        }

        public override dynamic Evaluate()
        {
            switch(this.Token.TokenType)
            {
                case TokenType.Plus: return this.LeftExpression.Evaluate() + this.RightExpression.Evaluate();
                case TokenType.Minus: return this.LeftExpression.Evaluate() - this.RightExpression.Evaluate();
                case TokenType.Multiplication: return this.LeftExpression.Evaluate() * this.RightExpression.Evaluate();
                case TokenType.Division: return this.LeftExpression.Evaluate() / this.RightExpression.Evaluate();
                default: throw new System.NotImplementedException();
            };
        }

        public override string GenerateCode()
        {
            return $"{this.LeftExpression.GenerateCode()} {this.Token.Lexeme} {this.RightExpression.GenerateCode()}";
        }

        // a + b
        public override Type GetExpressionType()
        {
            var leftType = LeftExpression.GetExpressionType();
            var rightType = RightExpression.GetExpressionType();
            if (_typeRules.TryGetValue((leftType, rightType, Token.TokenType), out var resultType))
            {
                return resultType;
            }

            throw new System.ApplicationException($"Cannot perform {Token.Lexeme} operation on types {leftType} and {rightType}");
        }
    }
}
