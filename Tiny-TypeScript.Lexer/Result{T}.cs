using System;
namespace Tiny_TypeScript.Lexer
{
    public class Result<T>
    {
        public T Value { get; set; }

        public Input Reminder { get; set; }


        internal Result(T value, Input reminder)
        {
            Value = value;
            Reminder = reminder;
        }

        internal Result(Input reminder)
        {
            Reminder = reminder;    
        }
    }
}
