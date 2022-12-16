﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyTypeScript.Lexer
{
    public class Scanner : IScanner
    {
        private Input input;
        private readonly Dictionary<string, TokenType> keywords;
        private readonly Dictionary<string, TokenType> types;

        public Scanner(Input input)
        {
            this.input = input;
            this.keywords = new Dictionary<string, TokenType>
            {
                { "if", TokenType.IfKeyword },
                { "else", TokenType.ElseKeyword },
                { "number", TokenType.NumberKeyword },
                { "string", TokenType.StringKeyword },
                { "while", TokenType.WhileKeyword },
                { "Console.log", TokenType.PrintKeyword },
                { "let", TokenType.LetKeyword},
                { "var", TokenType.VarKeyword },
                { "const", TokenType.ConstKeyword },
                { "boolean", TokenType.Boolean },
                { "string[]", TokenType.StringArrayKeyword },
                { "boolean[]", TokenType.BooleanArrayKeyword},
                { "number[]", TokenType.NumberArrayKeyword},
                { "for", TokenType.ForKeyword},
                { "function", TokenType.Function}
                
            };
            this.types = new Dictionary<string, TokenType> 
            {
                { "string", TokenType.StringKeyword },
                { "number", TokenType.NumberKeyword },
                { "boolean", TokenType.Boolean },
            };
        }

        public Token GetNextToken()
        {
            var lexeme = new StringBuilder();
            var currentChar = GetNextChar();
            while (true)
            {
                while (char.IsWhiteSpace(currentChar) || currentChar == '\n')
                {
                    currentChar = GetNextChar();
                }
                if (char.IsLetter(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsLetterOrDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }
                    if (this.types.ContainsKey(lexeme.ToString()))
                    {
                        currentChar = PeekNextChar();
                        if(currentChar== '[')
                        {
                           currentChar= GetNextChar();
                            lexeme.Append(currentChar);
                            currentChar = PeekNextChar();
                            if (currentChar == ']')
                            {
                                currentChar = GetNextChar();
                                lexeme.Append(currentChar);
                            }
                        }

                        
                    }
                    if (this.keywords.ContainsKey(lexeme.ToString()))
                    {
                        return lexeme.ToToken(input, this.keywords[lexeme.ToString()]);
                    }

                    return lexeme.ToToken(input, TokenType.Identifier);
                }
                else if (char.IsDigit(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }

                    if (currentChar != '.')
                    {
                        return lexeme.ToToken(input, TokenType.IntConstant);
                    }

                    currentChar = GetNextChar();
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }
                    return lexeme.ToToken(input, TokenType.FloatConstant);
                }
                else switch (currentChar)
                    {
                        case '/':
                            {
                                currentChar = PeekNextChar();
                                if (currentChar != '*')
                                {
                                    lexeme.Append(currentChar);
                                    return lexeme.ToToken(input, TokenType.Division);
                                }
                                while (true)
                                {
                                    currentChar = GetNextChar();
                                    while (currentChar == '*')
                                    {
                                        currentChar = GetNextChar();
                                    }

                                    if (currentChar == '/')
                                    {
                                        currentChar = GetNextChar();
                                        break;
                                    }
                                }
                                break;
                            }
                        case '<':
                            lexeme.Append(currentChar);
                            var nextChar = PeekNextChar();
                            switch (nextChar)
                            {
                                case '=':
                                    GetNextChar();
                                    lexeme.Append(nextChar);
                                    return lexeme.ToToken(input, TokenType.LessOrEqualThan);
                                case '>':
                                    lexeme.Append(nextChar);
                                    currentChar = GetNextChar();
                                    return lexeme.ToToken(input, TokenType.NotEqual);
                                default:
                                    return lexeme.ToToken(input, TokenType.LessThan);
                            }
                        case '>':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar != '=')
                            {
                                return lexeme.ToToken(input, TokenType.GreaterThan);
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return lexeme.ToToken(input, TokenType.GreaterOrEqualThan);
                        case '+':
                            nextChar= PeekNextChar();
                            if (nextChar != '+')
                            {
                                return lexeme.ToToken(input, TokenType.Plus);
                            }
                            lexeme.Append(nextChar);
                            GetNextChar();
                            return lexeme.ToToken(input, TokenType.PlusPlus);
                        case '-':
                            lexeme.Append(currentChar);
                            nextChar= PeekNextChar();
                            if (nextChar != '-')
                            {
                                return lexeme.ToToken(input, TokenType.Minus);
                            }
                            lexeme.Append(nextChar);
                            GetNextChar();
                            return lexeme.ToToken(input, TokenType.MinusMinus);
                        case '(':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.LeftParens);
                        case ')':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.RightParens);
                        case '*':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.Asterisk);
                        case ';':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.SemiColon);
                        case '=':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.Assignation);
                        case ':':
                            {
                                lexeme.Append(currentChar);
                                return lexeme.ToToken(input, TokenType.Colon);
                            }
                        case '\'':
                            {
                                lexeme.Append(currentChar);
                                currentChar = GetNextChar();
                                while (currentChar != '\'')
                                {
                                    lexeme.Append(currentChar);
                                    currentChar = GetNextChar();
                                }
                                lexeme.Append(currentChar);
                                return lexeme.ToToken(input, TokenType.StringConstant);
                            }
                        case '\"':
                            {
                                lexeme.Append(currentChar);
                                currentChar = GetNextChar();
                                while (currentChar != '\"')
                                {
                                    lexeme.Append(currentChar);
                                    currentChar = GetNextChar();
                                }
                                lexeme.Append(currentChar);
                                return lexeme.ToToken(input, TokenType.StringConstant);
                            }
                        case '\0':
                            return lexeme.ToToken(input, TokenType.EOF);
                        case '[':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.SquareOpenBrace);
                        case ']':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.SquareCloseBrace);
                        case '{':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.OpenBrace);
                        case '}':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.CloseBrace);
                        case ',':
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.Comma);
                        default:
                            throw new ApplicationException($"Caracter {lexeme} invalido en la columna: {input.Position.Column}, fila: {input.Position.Line}");
                    }
            }
        }

        private char GetNextChar()
        {
            var next = input.NextChar();
            input = next.Reminder;
            return next.Value;
        }

        private char PeekNextChar()
        {
            var next = input.NextChar();
            return next.Value;
        }
    }
}
