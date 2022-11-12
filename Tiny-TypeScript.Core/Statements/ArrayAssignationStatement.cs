using System;
using Tiny_TypeScript.Core.Expressions;

namespace Tiny_TypeScript.Core.Statements
{
    public class ArrayAssignationStatement : Statement
    {
        public IdExpression Id { get; }

        public TypedExpression Index { get; }

        public TypedExpression Expression { get; }

        private readonly TypedExpression _access;

        public ArrayAssignationStatement(ArrayAccessExpression access, TypedExpression expression)
        {
            Id = access.Id;
            Index = access.Index;
            Expression = expression;
            _access = access;
            this.ValidateSemantic();
        }

        public override void ValidateSemantic()
        {
            if (_access.GetExpressionType() is Core.Types.Array || Expression.GetExpressionType() is Core.Types.Array)
            {
               throw new ApplicationException($"Type {Expression.GetExpressionType()} is not assignable to {Id.GetExpressionType()}");
            }
            else if (_access.GetExpressionType() != Expression.GetExpressionType())
            {
                throw new ApplicationException($"Type {Expression.GetExpressionType()} is not assignable to {Id.GetExpressionType()}");
            }
        }

        public override string GenerateCode()
        {
            return $"{this.Id.GenerateCode()}[{this.Index.GenerateCode()}] = {this.Expression.GenerateCode()};{System.Environment.NewLine}";
        }

        public override void Interpret()
        {
            var symbol = this.Id.Evaluate();
            var position = this.Index.Evaluate();
            symbol[(int)position] = this.Expression.Evaluate();
            EnvironmentManager.UpdateSymbol(this.Id.Token.Lexeme, symbol);
        }
    }
}
