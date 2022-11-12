using System;
using System.Collections.Generic;
using System.Text;
using Tiny_TypeScript.Core;
using Tiny_TypeScript.Core.Interfaces;
using Tiny_TypeScript.Core.Models;

namespace Tiny_TypeScript.Lexer
{
    public class Scanner : IScanner
    {
        private Input input;
        private readonly ILogger logger;
        private readonly Dictionary<string, TokenType> keywords;

        public Scanner(Input input, ILogger logger)
        {
            this.input = input;
            this.logger = logger;
            this.keywords = new Dictionary<string, TokenType>()
            {
                ["if"] = TokenType.IfKeyword,
                ["else"] = TokenType.ElseKeyword,
                ["number"] = TokenType.NumberKeyword,
                ["string"] = TokenType.StringKeyword,
                ["while"] = TokenType.WhileKeyword,
                ["Array"] = TokenType.ArrayKeyword,
                ["print"] = TokenType.PrintKeyword,
                ["bool"] = TokenType.BoolKeyword,
                ["true"] = TokenType.TrueKeyword,
                ["false"] = TokenType.FalseKeyword,
            };
        }

        public Token GetNextToken()
        {
            var lexeme = new StringBuilder();
            var currentChar = this.GetNextChar();
            while (currentChar != '\0')
            {
                while (char.IsWhiteSpace(currentChar) || currentChar == '\n')
                {
                    currentChar = this.GetNextChar();
                }

                if (char.IsLetter(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = this.PeekNextChar();
                    while (char.IsLetterOrDigit(currentChar))
                    {
                        currentChar = this.GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = this.PeekNextChar();
                    }

                    var tokenLexeme = lexeme.ToString();
                    if (this.keywords.ContainsKey(tokenLexeme))
                    {
                        return BuildToken(tokenLexeme, this.keywords[tokenLexeme]);
                    }

                    return BuildToken(tokenLexeme, TokenType.Identifier);
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

                    if (currentChar == '.')
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                        while (char.IsDigit(currentChar))
                        {
                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            currentChar = PeekNextChar();
                        }
                    }

                    return BuildToken(lexeme.ToString(), TokenType.NumberLiteral);
                }

                switch (currentChar)
                {
                    case '\0':
                        return BuildToken("\0", TokenType.EOF);
                    case '+':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Plus);
                    case '-':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Minus);
                    case '*':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Multiplication);
                    case '/':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Division);
                    case '<':
                        lexeme.Append(currentChar);
                        var nextChar = this.PeekNextChar();
                        if (nextChar == '=')
                        {
                            currentChar = this.GetNextChar();
                            lexeme.Append(currentChar);
                            return BuildToken(lexeme.ToString(), TokenType.LessOrEqualThan);
                        }
                        return BuildToken(lexeme.ToString(), TokenType.LessThan);
                    case '>':
                        lexeme.Append(currentChar);
                        nextChar = this.PeekNextChar();
                        if (nextChar == '=')
                        {
                            currentChar = this.GetNextChar();
                            lexeme.Append(currentChar);
                            return BuildToken(lexeme.ToString(), TokenType.GreaterOrEqualThan);
                        }
                        return BuildToken(lexeme.ToString(), TokenType.GreaterThan);
                    case ':':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Colon);
                    case ';':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Semicolon);
                    case '(':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.LeftParens);
                    case ')':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.RightParens);
                    case '[':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.LeftBracket);
                    case ']':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.RightBracket);
                    case '{':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.LeftBrace);
                    case '}':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.RightBrace);
                    case ',':
                        lexeme.Append(currentChar);
                        return BuildToken(lexeme.ToString(), TokenType.Comma);
                    case '=':
                        lexeme.Append(currentChar);
                        nextChar = this.PeekNextChar();
                        if (nextChar == '=')
                        {
                            currentChar = this.GetNextChar();
                            lexeme.Append(currentChar);
                            return BuildToken(lexeme.ToString(), TokenType.Equal);
                        }
                        break;
                    case '\'':
                        lexeme.Append(currentChar);
                        currentChar = GetNextChar();
                        while (currentChar != '\'' && currentChar != '\0')
                        {
                            lexeme.Append(currentChar);
                            currentChar = GetNextChar();
                        }
                        if (currentChar == '\'')
                        {
                            lexeme.Append(currentChar);
                            return BuildToken(lexeme.ToString(), TokenType.StringLiteral);
                        }
                        break;
                    case '&':
                        lexeme.Append(currentChar);
                        currentChar = GetNextChar();
                        if (currentChar == '&')
                        {
                            lexeme.Append(currentChar);
                            return BuildToken(lexeme.ToString(), TokenType.LogicalAnd);
                        }
                        lexeme.Clear();
                        logger.Error($"Expected & but {currentChar} was found, line ine: {this.input.Position.Line} and column: {this.input.Position.Column}");
                        continue;
                    case '|':
                        lexeme.Append(currentChar);
                        currentChar = GetNextChar();
                        if (currentChar == '|')
                        {
                            lexeme.Append(currentChar);
                            return BuildToken(lexeme.ToString(), TokenType.LogicalOr);
                        }
                        lexeme.Clear();
                        logger.Error($"Expected | but {currentChar} was found, line ine: {this.input.Position.Line} and column: {this.input.Position.Column}");
                        continue;
                    default:
                        break;
                }

                logger.Error($"Invalid character {currentChar} in line: {this.input.Position.Line} and column: {this.input.Position.Column}");
                currentChar = this.GetNextChar();
            }
            return BuildToken("\0", TokenType.EOF);
        }

        private Token BuildToken(string lexeme, TokenType tokenType)
        {
            return new Token
            {
                Column = this.input.Position.Column > 0 ? this.input.Position.Column - 1 : this.input.Position.Column,
                Line = this.input.Position.Line + 1,
                Lexeme = lexeme,
                TokenType = tokenType,
            };
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
