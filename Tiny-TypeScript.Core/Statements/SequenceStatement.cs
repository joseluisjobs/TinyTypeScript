using System;
namespace Tiny_TypeScript.Core.Statements
{
    public class SequenceStatement : Statement
    {
        public SequenceStatement(Statement firstStatement, Statement nextStatement)
        {
            FirstStatement = firstStatement;
            NextStatement = nextStatement;
            this.ValidateSemantic();    
        }

        public Statement FirstStatement { get; }
        public Statement NextStatement { get; }

        public override string GenerateCode()
        {
            return $"{this.FirstStatement?.GenerateCode()}{System.Environment.NewLine}{this.NextStatement?.GenerateCode()}";
        }

        public override void Interpret()
        {
            this.FirstStatement?.Interpret();
            this.NextStatement?.Interpret();
        }

        public override void ValidateSemantic()
        {
            this.FirstStatement?.ValidateSemantic();
            this.NextStatement?.ValidateSemantic();
        }
    }
}
