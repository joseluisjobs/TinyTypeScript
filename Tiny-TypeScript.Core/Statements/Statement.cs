using System;
namespace Tiny_TypeScript.Core.Statements
{
    public abstract class Statement
    {
        public abstract void ValidateSemantic();

        public abstract string GenerateCode();

        public abstract void Interpret();
    }
}
