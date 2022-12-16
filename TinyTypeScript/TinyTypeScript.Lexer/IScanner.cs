using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyTypeScript.Lexer
{
    public interface IScanner
    {
        Token GetNextToken();
    }
}
