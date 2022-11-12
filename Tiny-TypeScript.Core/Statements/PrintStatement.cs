using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiny_TypeScript.Core.Expressions;

namespace Tiny_TypeScript.Core.Statements
{
    public class PrintStatement : Statement
    {
        public PrintStatement(IEnumerable<TypedExpression> parameters)
        {
            Parameters = parameters;
            this.ValidateSemantic();
        }

        public IEnumerable<TypedExpression> Parameters { get; }

        public override string GenerateCode()
        {
            var code = "cout";
            foreach (var param in this.Parameters)
            {
                code += $"<<{param.GenerateCode()}";
            }

            code += "<<endl;";
            return code;
        }

        public override void Interpret()
        {
            foreach (var param in this.Parameters)
            {
                var value = param.Evaluate();
                if (value != null)
                {
                    Console.WriteLine(value);
                }
            }
        }

        public override void ValidateSemantic()
        {
            if (Parameters.Any(x => x.GetExpressionType() != Types.Type.String))
            {
                throw new ApplicationException("All parameters for print method must be string");
            }
        }
    }
}
