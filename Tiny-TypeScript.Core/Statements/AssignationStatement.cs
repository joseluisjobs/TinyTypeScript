using System;
using Tiny_TypeScript.Core.Expressions;

namespace Tiny_TypeScript.Core.Statements
{
    public class AssignationStatement : Statement
    {
        public AssignationStatement(IdExpression id, TypedExpression expression)
        {
            Id = id;
            Expression = expression;
            this.ValidateSemantic();
        }

        public IdExpression Id { get; }
        public TypedExpression Expression { get; }

        public override string GenerateCode()
        {
            return $"{this.Id.GenerateCode()} = {this.Expression.GenerateCode()};{System.Environment.NewLine}";
        }

        public override void Interpret()
        {
            var value = this.Expression.Evaluate();
            EnvironmentManager.UpdateSymbol(this.Id.Token.Lexeme, value);
        }

        public override void ValidateSemantic()
        {
            if (this.Id.GetExpressionType() != Expression.GetExpressionType())
            {
                throw new ApplicationException($"Type {Expression.GetExpressionType()} is not assignable to {Id.GetExpressionType()}");
            }
        }
    }
}
