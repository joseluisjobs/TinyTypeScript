using System;
namespace Tiny_TypeScript.Core.Types
{
    public class Array : Type
    {
        public Type Of { get; }

        public int Size { get; set; }

        public Array(string lexeme, TokenType tokenType, Type of, int size)
            : base(lexeme, tokenType)
        {
            Of = of;
            Size = size;
        }
    }
}
