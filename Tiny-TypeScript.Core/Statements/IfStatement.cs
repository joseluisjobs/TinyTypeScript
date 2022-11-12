using System;
using Tiny_TypeScript.Core.Expressions;

namespace Tiny_TypeScript.Core.Statements
{
    public class IfStatement : Statement
    {
        public IfStatement(TypedExpression expression, Statement trueStatement, Statement falseStatement)
        {
            Expression = expression;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
            this.ValidateSemantic();
        }

        public TypedExpression Expression { get; }
        public Statement TrueStatement { get; }
        public Statement FalseStatement { get; }

        public override string GenerateCode()
        {
            var code = $"if({this.Expression.GenerateCode()}){{";
            code += this.TrueStatement.GenerateCode();
            if (this.FalseStatement != null)
            {
                code += $"}}else{{{System.Environment.NewLine}{this.FalseStatement.GenerateCode()}{System.Environment.NewLine}";
            }
            code += "}";
            return code;
        }

        public override void Interpret()
        {
            if (this.Expression.Evaluate())
            {
                this.TrueStatement.Interpret();
            }
            else
            {
                this.FalseStatement?.Interpret();
            }
        }

        public override void ValidateSemantic()
        {
            if (this.Expression.GetExpressionType() != Types.Type.Bool)
            {
                throw new ApplicationException($"Expression inside if must be boolean");
            }
        }
    }
}
