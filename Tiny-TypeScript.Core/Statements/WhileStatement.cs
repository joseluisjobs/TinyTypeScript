using System;
using Tiny_TypeScript.Core.Expressions;
using Tiny_TypeScript.Core.Types;

namespace Tiny_TypeScript.Core.Statements
{
    public class WhileStatement : Statement
    {
        public WhileStatement(TypedExpression expression, Statement statement)
        {
            Expression = expression;
            Statement = statement;
            this.ValidateSemantic();
        }

        public TypedExpression Expression { get; }
        public Statement Statement { get; }

        public override string GenerateCode()
        {
            var code = $"while({this.Expression.GenerateCode()}){{ {System.Environment.NewLine}";
            code += this.Statement.GenerateCode();
            code += System.Environment.NewLine;
            code += "}";
            return code;
        }

        public override void Interpret()
        {
            while (this.Expression.Evaluate())
            {
                this.Statement.Interpret();
            }
        }

        public override void ValidateSemantic()
        {
            if (this.Expression.GetExpressionType() != Types.Type.Bool)
            {
                throw new ApplicationException($"Expression inside while must be boolean");
            }
        }
    }
}
