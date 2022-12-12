<<<<<<< Updated upstream
﻿using LanguagueCompiler.Lexer;
=======
﻿using LanguageCompiler.Core;
using LanguagueCompiler.Lexer;

using Microsoft.VisualBasic;
>>>>>>> Stashed changes

namespace LanguageCompiler.Parser
{

    public class Parser
    {
        private readonly IScanner scanner;
        private Token lookAhead;
        public Parser(IScanner scanner)
        {
            this.scanner = scanner;
            this.Move();
        }

<<<<<<< Updated upstream
        private void Code()
        {
            Block();
        }

        private void Block()
        {
            Decls();
            Stmts();
        }

        private void Decls()
        {
            if (this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword || this.lookAhead.TokenType == TokenType.ConstKeword)
            {
                Decl();
                Decls();
            }
            
        }

        private void Decl()
=======
        public Statement Parse()
        {
            return Program();
        }

        private Statement Program()
        {
            if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            {
                Element();
                Program();
            }
        }

        private Statement Element()
        {
            return Statement();
        }

        private Statement CompoundStatement()
        {
            Match(TokenType.OpenBrace);
            var statement = Statements();
            Match(TokenType.CloseBrace);
            return statement;
        }

        private Statement Statements()
        {
            if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            {
                return new SequenceStatement(Statement(), Statements());
            }
            return null;
        }

        public Statement Statement()
>>>>>>> Stashed changes
        {
           switch(this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
<<<<<<< Updated upstream
                    VariableDecl();
                    Match(TokenType.Identifier);
                    Match(TokenType.Colon);
                    _Type();
                    Match(TokenType.SemiColon);
                    break;
                case TokenType.LetKeword:
                    VariableDecl();
                    Match(TokenType.Identifier);
                    Match(TokenType.Colon);
                    _Type();
                    Match(TokenType.SemiColon);
                    break;
                case TokenType.ConstKeword:
                    VariableDecl();
                    Match(TokenType.Identifier);
                    Match(TokenType.Colon);
                    _Type();
                    Match(TokenType.SemiColon);
=======
                    return Variables();
                    break;
                case TokenType.LetKeword:
                    return Variables();
                    break;
                case TokenType.ConstKeword:
                    return Function();
                    break;
                case TokenType.IfKeword:
                    return IfStatement();
                    break;
                case TokenType.WhileKeword:
                    return WhileStatement();
                    break;
                case TokenType.ConsoleKeword:
                    return PrintStatement();
                    break;
                case TokenType.ForKeword:
                    return ForStatement();
                    break;
                case TokenType.ForeachKeword:
                    return ForeachStatement();
                    break;
                // no se si lo de comentarios viene aca
                case TokenType.ReturnKeword:
                    return ReturnStatement();
                    break;
                case TokenType.ContinueKeword:
                    return ContinueStatement();
                    break;
                case TokenType.BreakKeword:
                    return BreakStatement();
>>>>>>> Stashed changes
                    break;
            }
        }

<<<<<<< Updated upstream
        private void _Type()
=======
        private Statement BreakStatement()
        {
            Match(TokenType.BreakKeword);
            Match(TokenType.SemiColon);
            return new BreakStatement();
        }

        private void ContinueStatement()
        {
            Match(TokenType.ContinueKeword);
            Match(TokenType.SemiColon);
        }

        private void ReturnStatement()
        {
            Match(TokenType.ReturnKeword);
            ReturnExpressionOpt();
            Match(TokenType.SemiColon);
        }

        private void ReturnExpressionOpt()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                LogicalOrExpress();
            }
        }

        private void ForeachStatement()
        {
            Match(TokenType.ForeachKeword);
            Match(TokenType.LeftParens);
            Variables();
            Match(TokenType.InKeword);
            Identifier();
            Match(TokenType.RightParens);
            CompoundStatement();
        }

        private void ForStatement()
        {
            Match(TokenType.ForKeword);
            Match(TokenType.LeftParens);
            Variables();
            Expression();
            Match(TokenType.SemiColon);
            Expression();
            Match(TokenType.RightParens);
            CompoundStatement();
        }

        private void PrintStatement()
        {
            Match(TokenType.ConsoleKeword);
            Match(TokenType.Decimal);
            Match(TokenType.LogKeword);
            Match(TokenType.LeftParens);
            LogicalOrExpress();
            Match(TokenType.RightParens);
        }

        private Expression LogicalOrExpress()
        {
            var expr = LogicalAndExpress();
            while (this.lookAhead.TokenType == TokenType.Or)
            {
                var token = this.lookAhead;
                Move();
                expr = new LogicalOrExpression(expr,LogicalAndExpress());
            }
            return expr;
        }

        private Expression LogicalAndExpress()
        {
            var expr = EqExpression();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                var token = this.lookAhead;
                Move();
                expr = new LogicalAndExpression(expr,EqExpression());
            }
            return expr;
        }

        private Expression EqExpression()
        {
            RelExpression();
            while (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
            {
                var token = this.lookAhead;
                Move();
                RelExpression();
            }
        }

        private void RelExpression()
        {
            Expression();
            while (this.lookAhead.TokenType == TokenType.LessThan || this.lookAhead.TokenType == TokenType.LessOrEqualThan ||
                this.lookAhead.TokenType == TokenType.GreaterThan || this.lookAhead.TokenType == TokenType.GreaterOrEqualThan)
            {
                var token = this.lookAhead;
                Move();
                Expression();
            }
        }

        private Statement WhileStatement()
        {
            Match(TokenType.WhileKeword);
            Match(TokenType.LeftParens);
            var expression = Expression();
            Match(TokenType.RightParens);
            var statement = CompoundStatement();
            return new WhileStatement(expression, statement);
        }

        private Expression Expression()
        {
            Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
            {
                var token = this.lookAhead;
                Move();
                Term();
            }
        }

        private void Term()
        {
            Identifier();
            while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Division)
            {
                var token = this.lookAhead;
                Move();
                Identifier();
            }
        }

        private void Identifier()
>>>>>>> Stashed changes
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.IntKeword:
                    Match(TokenType.IntKeword);
                    break;
                case TokenType.StringConstant:
                    Match(TokenType.StringKeword);
                    break;
                case TokenType.BoolKeword:
                    Match(TokenType.BoolKeword);
                    break;
            }
        }

<<<<<<< Updated upstream
        private void VariableDecl()
=======
        private Statement IfStatement()
        {
            Match(TokenType.IfKeword);
            Match(TokenType.LeftParens);
            var logical = LogicalOrExpress();
            Match(TokenType.RightParens);
            var compound = CompoundStatement();
            var elseStatement = ElseStatement();
            return new IfStatement(logical, compound, elseStatement);
        }

        private Statement ElseStatement()
        {
            if (this.lookAhead.TokenType == TokenType.ElseKeword)
            {
                return Statement();
            }
            return null;
        }

        public void Variables()
>>>>>>> Stashed changes
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
                     Match(TokenType.VarKeword);
                     break;
                case TokenType.LetKeword:
                     Match(TokenType.LetKeword);
                     break;
                case TokenType.ConstKeword:
                     Match(TokenType.ConstKeword);
                     break;
            }
        }

        private void Stmts()
        {
            if (this.lookAhead.TokenType==TokenType.Identifier)
            {
                Stmt();
                Stmts();
            }
            
        }

        private void Stmt()
        {
            Match(TokenType.Identifier);
            switch(this.lookAhead.TokenType)
            {
                case TokenType.IfKeword:
                    IfStmt();
                    return;
                case TokenType.WhileKeword:
                    WhileStmt();
                    return;
                case TokenType.ConsoleKeword:
                    PrintStmt();
                    return;
                case TokenType.ForKeword:
                    ForStmt();
                    return;

            }
        }

        private void Move()
        {
            this.lookAhead=this.scanner.GetNextToken();
        }

        private void Match(TokenType tokenType)
        {
            if(this.lookAhead.TokenType!=tokenType)
            {
                throw new ApplicationException($"Syntax error! Expected {tokenType} but found {this.lookAhead.TokenType}.");
            }
            this.Move();
        }
    }
}