using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tiny_TypeScript.Core.Statements
{
    public class BlockStatement : Statement
    {
        private readonly Dictionary<string, string> _typeMapping;
        public BlockStatement(Statement statement)
        {
            Statement = statement;
            _typeMapping = new Dictionary<string, string>
            {
                { "number", "int" },
                { "string", "string" },
                { "bool", "bool" },
            };
            this.ValidateSemantic();
        }

        public Statement Statement { get; }

        public override string GenerateCode()
        {
            var code = string.Empty;
            foreach (var symbol in EnvironmentManager.GetSymbolsForCurrentContext())
            {
                var symbolType = symbol.Id.GetExpressionType();
                if (symbolType is Types.Array array)
                {
                    symbolType = array.Of;
                    code += $"vector<{_typeMapping[symbolType.Lexeme]}> {symbol.Id.Token.Lexeme}({array.Size});{System.Environment.NewLine}";
                }
                else
                {
                    code += $"{_typeMapping[symbolType.Lexeme]} {symbol.Id.Token.Lexeme};{System.Environment.NewLine}";
                }
            }

            code += this.Statement.GenerateCode();
            return code;
        }

        public override void Interpret()
        {
            var elements = EnvironmentManager.GetSymbolsUnassignedSymbolsInterpretation().ToList();
            for (int i = 0; i < elements.Count; i++)
            {
                var symbol = elements[i];
                var symbolType = symbol.Id.GetExpressionType();
                if (symbolType is Types.Array array)
                {
                    var dotnetType = _typeMapping[array.Of.Lexeme];
                    switch (dotnetType)
                    {
                        case "int": EnvironmentManager.UpdateSymbol(symbol.Id.Token.Lexeme, new int[array.Size]);
                            break;
                        case "string": EnvironmentManager.UpdateSymbol(symbol.Id.Token.Lexeme, new string[array.Size]);
                            break;
                        default:
                            break;
                    }
                }
            }
            this.Statement?.Interpret();
        }

        public override void ValidateSemantic()
        {
            this.Statement?.ValidateSemantic();
        }
    }
}
