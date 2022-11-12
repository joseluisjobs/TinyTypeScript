using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tiny_TypeScript.Core;
using Tiny_TypeScript.Core.Expressions;
using Tiny_TypeScript.Core.Interfaces;
using Tiny_TypeScript.Core.Models;
using Tiny_TypeScript.Core.Statements;

namespace Tiny_TypeScript.Parser
{
    //LL(1)
    public class Parser : IParser
    {
        private readonly IScanner scanner;
        private readonly ILogger logger;
        private Token lookAhead;

        public Parser(IScanner scanner, ILogger logger)
        {
            this.scanner = scanner;
            this.logger = logger;
            this.Move();
        }

        public Statement Parse()
        {
            return Program();
        }

        private Statement Program()
        {
            return Block();
        }

        private Statement Block()
        {
            //{
            this.Match(TokenType.LeftBrace);
            EnvironmentManager.PushContext();
            Decls();
            if (this.lookAhead.TokenType == TokenType.Semicolon)
            {
                //;
                this.Match(TokenType.Semicolon);
            }
            var statements = Stmts();
            //}
            this.Match(TokenType.RightBrace);
            var blockStatement = new BlockStatement(statements);
            EnvironmentManager.PopContext();
            return blockStatement;
        }

        private void Decls()
        {
            while (this.lookAhead.TokenType == TokenType.Identifier)
            {
                Decl();
            }
        }

        private void Decl()
        {
            //id
            var token = this.lookAhead;
            this.Match(TokenType.Identifier);
            //:
            this.Match(TokenType.Colon);
            var type = Type();         
            var id = new IdExpression(type, token);
            EnvironmentManager.Put(token.Lexeme, id, null);
        }

        private Core.Types.Type Type()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.NumberKeyword:
                    this.Match(TokenType.NumberKeyword);
                    return Core.Types.Type.Number;
                case TokenType.StringKeyword:
                    this.Match(TokenType.StringKeyword);
                    return Core.Types.Type.String;
                case TokenType.ArrayKeyword:
                    this.Match(TokenType.ArrayKeyword);
                    this.Match(TokenType.LessThan);
                    var type = Type();
                    this.Match(TokenType.GreaterThan);
                    this.Match(TokenType.LeftParens);
                    var size = this.lookAhead;
                    this.Match(TokenType.NumberLiteral);
                    this.Match(TokenType.RightParens);
                    return new Core.Types.Array("[]", TokenType.ComplexType, type, int.Parse(size.Lexeme));
                case TokenType.BoolKeyword:
                    this.Match(TokenType.BoolKeyword);
                    return Core.Types.Type.Bool;
                default:
                    throw new ApplicationException($"Syntax error! Unrecognized type in line: {this.lookAhead.Line} and column: {this.lookAhead.Column}");
            }
        }

        private Statement Stmts()
        {
            if (this.lookAhead.TokenType == TokenType.RightBrace)
            {
                return null;
            }

            return new SequenceStatement(Stmt(), Stmts());
        }

        private Statement Stmt()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.Identifier:
                    var symbol = EnvironmentManager.Get(this.lookAhead.Lexeme);
                    this.Match(TokenType.Identifier);
                    TypedExpression index = null;
                    if (this.lookAhead.TokenType == TokenType.LeftBracket)
                    {
                        this.Match(TokenType.LeftBracket);
                        index = LogicalOrExpr();
                        this.Match(TokenType.RightBracket);
                    }

                    this.Match(TokenType.LessThan);
                    this.Match(TokenType.Minus);
                    var stmt = AssignmentStmt(symbol.Id, index);
                    this.Match(TokenType.Semicolon);
                    return stmt;
                case TokenType.IfKeyword:
                    this.Match(TokenType.IfKeyword);
                    this.Match(TokenType.LeftParens);
                    var TypedExpression = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    var trueStatement = Stmt();
                    if (this.lookAhead.TokenType != TokenType.ElseKeyword)
                    {
                        return new IfStatement(TypedExpression, trueStatement, null);
                    }
                    this.Match(TokenType.ElseKeyword);
                    var falseStatement = Stmt();
                    return new IfStatement(TypedExpression, trueStatement, falseStatement);
                case TokenType.WhileKeyword:
                    this.Match(TokenType.WhileKeyword);
                    this.Match(TokenType.LeftParens);
                    TypedExpression = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    return new WhileStatement(TypedExpression, Stmt());
                case TokenType.PrintKeyword:
                    this.Match(TokenType.PrintKeyword);
                    this.Match(TokenType.LeftParens);
                    var @params = Params();
                    this.Match(TokenType.RightParens);
                    this.Match(TokenType.Semicolon);
                    return new PrintStatement(@params);
                default:
                    return Block();
            }
        }

        private IEnumerable<TypedExpression> Params()
        {
            var @params = new List<TypedExpression>();
            @params.Add(LogicalOrExpr());
            @params.AddRange(ParamsPrime());
            return @params;
        }

        private IEnumerable<TypedExpression> ParamsPrime()
        {
            var @params = new List<TypedExpression>();
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                this.Match(TokenType.Comma);
                @params.Add(LogicalOrExpr());
                @params.AddRange(ParamsPrime());
            }
            return @params;
        }

        private Statement AssignmentStmt(IdExpression id, TypedExpression index)
        {
            var expr = LogicalOrExpr();
            if (index == null)
            {
                return new AssignationStatement(id, expr);
            }

            var type = ((Core.Types.Array)id.GetExpressionType()).Of;
            var access = new ArrayAccessExpression(type, this.lookAhead, id, index);
            return new ArrayAssignationStatement(access, expr);
        }

        private TypedExpression LogicalOrExpr()
        {
            var TypedExpression = LogicalAndExpr();
            while (this.lookAhead.TokenType == TokenType.LogicalOr)
            {
                var token = this.lookAhead;
                this.Move();
                TypedExpression =  new LogicalExpression(token, TypedExpression, LogicalAndExpr());
            }

            return TypedExpression;
        }

        private TypedExpression LogicalAndExpr()
        {
            var expr = Eq();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new LogicalExpression(token, expr, Eq());
            }

            return expr;
        }

        private TypedExpression Eq()
        {
            var expr = Rel();
            while (this.lookAhead.TokenType == TokenType.Equal)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new RelationalExpression(token, expr, Rel());
            }

            return expr;
        }


        private TypedExpression Rel()
        {
            var expr = Expr();
            while(this.lookAhead.TokenType == TokenType.LessThan ||
                this.lookAhead.TokenType == TokenType.GreaterThan ||
                this.lookAhead.TokenType == TokenType.LessOrEqualThan ||
                this.lookAhead.TokenType == TokenType.GreaterOrEqualThan)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new RelationalExpression(token, expr, Expr());
            }

            return expr;
        }

        private TypedExpression Expr()
        {
            var expr = Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new ArithmeticExpression(token, expr, Term());
            }

            return expr;
        }

        private TypedExpression Term()
        {
            var expr = PostFixExpr();
            while (this.lookAhead.TokenType == TokenType.Multiplication || this.lookAhead.TokenType == TokenType.Division)
            {
                var token = this.lookAhead;
                this.Move();
                expr = new ArithmeticExpression(token, expr, PostFixExpr());
            }

            return expr;
        }

        private TypedExpression PostFixExpr()
        {
            var expr = Factor();
            if (this.lookAhead.TokenType == TokenType.LeftBracket)
            {
                var id = expr as IdExpression;
                this.Match(TokenType.LeftBracket);
                var index = LogicalOrExpr();
                this.Match(TokenType.RightBracket);

                var type = ((Core.Types.Array)id.GetExpressionType()).Of;
                return new ArrayAccessExpression(type, this.lookAhead, id, index);
            }

            return expr;
        }

        private TypedExpression Factor()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.LeftParens:
                    this.Match(TokenType.LeftParens);
                    var expr = LogicalOrExpr();
                    this.Match(TokenType.RightParens);
                    return expr;
                case TokenType.NumberLiteral:
                    var token = this.lookAhead;
                    this.Match(TokenType.NumberLiteral);
                    return new ConstantExpression(Core.Types.Type.Number, token);
                case TokenType.StringLiteral:
                    token = this.lookAhead;
                    this.Match(TokenType.StringLiteral);
                    return new ConstantExpression(Core.Types.Type.String, token);
                case TokenType.TrueKeyword:
                    token = this.lookAhead;
                    this.Match(TokenType.TrueKeyword);
                    return new ConstantExpression(Core.Types.Type.Bool, token);
                case TokenType.FalseKeyword:
                    token = this.lookAhead;
                    this.Match(TokenType.FalseKeyword);
                    return new ConstantExpression(Core.Types.Type.Bool, token);
                default:
                    token = this.lookAhead;
                    this.Match(TokenType.Identifier);
                    return EnvironmentManager.Get(token.Lexeme).Id;
            }
        }
    
        private void Move()
        {
            this.lookAhead = this.scanner.GetNextToken();
        }

        private void Match(TokenType expectedTokenType)
        {
            if (this.lookAhead.TokenType != expectedTokenType)
            {
                this.logger.Error($"Syntax Error! expected token {expectedTokenType} but found {this.lookAhead.TokenType} on line {this.lookAhead.Line} and column {this.lookAhead.Column}");
                throw new ApplicationException($"Syntax Error! expected token {expectedTokenType} but found {this.lookAhead.TokenType} on line {this.lookAhead.Line} and column {this.lookAhead.Column}");
            }
            this.Move();
        }
    }
}
