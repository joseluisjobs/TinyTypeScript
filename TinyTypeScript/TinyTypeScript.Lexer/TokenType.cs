using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyTypeScript.Lexer
{
    public enum TokenType
    {
        Asterisk,
        Plus,
        Minus,
        LeftParens,
        RightParens,
        SemiColon,
        Equal,
        Division,
        LessThan,
        LessOrEqualThan,
        NotEqual,
        GreaterThan,
        GreaterOrEqualThan,
        NumberKeyword,
        IfKeyword,
        ElseKeyword,
        Identifier,
        IntConstant,
        FloatConstant,
        Assignation,
        StringConstant,
        EOF,
        OpenBrace,
        CloseBrace,
        Comma,
        StringKeyword,
        StringArrayKeyword,
        NumberArrayKeyword,
        BooleanArrayKeyword,
        Colon,
        WhileKeyword,
        PrintKeyword,
        LogicalOr,
        LogicalAnd,
        BreakKeyword,
        ConstKeyword,
        DoKeyword,
        FalseKeyword,
        TrueKeyword,
        InKeyword,
        ForKeyword,
        ReturnKeyword,
        VarKeyword,
        VoidKeyword,
        LetKeyword,
        OfKeyWord,
        ContinueKeyword,
        LogicalEqual,
        SquareOpenBrace,
        SquareCloseBrace,
        CircumFlex,
        PlusPlus,
        MinusMinus,
        Boolean,


    }
}
