using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Expressions
{
    public class ConstantExpression : TypedExpression
    {
        public ConstantExpression(Type type, Token token)
            : base(type, token)
        {
        }

        public override dynamic Evaluate()
        {
            switch(this.Token.TokenType)
            {
                case TokenType.NumberLiteral: return int.TryParse(this.Token.Lexeme, out var result) ?  result : float.Parse(this.Token.Lexeme);
                case TokenType.StringLiteral: return this.Token.Lexeme.Replace("'", "\"");
                case TokenType.TrueKeyword: return bool.Parse(this.Token.Lexeme);
                case TokenType.FalseKeyword: return bool.Parse(this.Token.Lexeme);
                default: throw new System.NotImplementedException();
            };
        }

        public override string GenerateCode()
        {
            if (this.type == Type.String)
            {
                return Token.Lexeme.Replace("'", "\"");
            }
            return Token.Lexeme;
        }

        public override Type GetExpressionType()
        {
            return type;
        }
    }
}
