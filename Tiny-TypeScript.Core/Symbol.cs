using System;
using Tiny_TypeScript.Core.Expressions;

namespace Tiny_TypeScript.Core
{
    public class Symbol
    {
        public Symbol(IdExpression id, dynamic value)
        {
            Id = id;
            Value = value;
        }

        public IdExpression Id { get; }

        public dynamic Value { get; set; }
    }
}
