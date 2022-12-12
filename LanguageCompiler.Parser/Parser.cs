using LanguageCompiler.Core;
using LanguagueCompiler.Lexer;
using Microsoft.VisualBasic;

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

        private void Element()
        {
            Statement();
        }

        private Statement CompoundStatement()
        {
            Match(TokenType.OpenBrace);
            Statements();
            Match(TokenType.CloseBrace);
        }

        private Statement Statements()
        {
            if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            {
                Statement();
                Statements();
                
            }
        }

        public Statement Statement()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
                    return Variables();
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
                    break;
            }
        }

        private void BreakStatement()
        {
            Match(TokenType.BreakKeword);
            Match(TokenType.SemiColon);
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

        private Statement ForStatement()
        {
            Match(TokenType.ForKeword);
            Match(TokenType.LeftParens);
            var vars = Variables();
            var expr1 = Express();
            Match(TokenType.SemiColon);
            var expr2 = Express();
            Match(TokenType.RightParens);
            var statement = CompoundStatement();
            return new ForStatement(vars, expr1, expr2, statement);
        }

        private void PrintStatement()
        {
            Match(TokenType.ConsoleKeword);
            Match(TokenType.Decimal);
            Match(TokenType.LogKeword);
            Match(TokenType.LeftParens);
            var @params = FunctionParams();
            Match(TokenType.RightParens);
            return new PrintStatement(@params)
        }

        private Expression LogicalOrExpress()
        {
            var expr = LogicalAndExpress();
            while (this.lookAhead.TokenType == TokenType.Or)
            {
                var token = this.lookAhead;
                Move();
                expr = new LogicalOrExpression(expr, LogicalAndExpress());
            }
            return expr;
        }

        private Expression LogicalAndExpress()
        {
            var expr = EqExpress();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                var token = this.lookAhead;
                Move();
                expr = new LogicalAndExpression(expr, EqExpress());
            }
            return expr;
        }

        private Expression EqExpress()
        {
            var expr = RelExpress();
            while (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
            {
                var token = this.lookAhead;
                Move();
                expr = new EqualExpression(expr, RelExpress(), token);
            }

            return expr;
        }

        private Expression RelExpress()
        {
            var expr = Express();
            while (this.lookAhead.TokenType == TokenType.LessThan || this.lookAhead.TokenType == TokenType.LessOrEqualThan ||
                this.lookAhead.TokenType == TokenType.GreaterThan || this.lookAhead.TokenType == TokenType.GreaterOrEqualThan)
            {
                var token = this.lookAhead;
                Move();
                Express();
                expr = new RelationalExpression(expr, Express(), token);
            }
            return expr;
        }

        private Statement WhileStatement()
        {
            Match(TokenType.WhileKeword);
            Match(TokenType.LeftParens);
            var expr = Express();
            Match(TokenType.RightParens);
            var statement = CompoundStatement();
            return new WhileStatement(expr, statement);
        }

        private Expression Express()
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
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.IntConstant:
                    Match(TokenType.IntConstant);
                    break;
                case TokenType.FloatConstant:
                    Match(TokenType.FloatConstant);
                    break;
                default:
                    Match(TokenType.Identifier);
                    break;
            }
        }

        private Statement IfStatement()
        {
            Match(TokenType.IfKeword);
            Match(TokenType.LeftParens);
            var expr = LogicalOrExpress();
            Match(TokenType.RightParens);
            var compoundStatement = CompoundStatement();
            var elseStatement= ElseStatement();
            return new IfStatement(expr, compoundStatement, elseStatement);
        }

        private Statement ElseStatement()
        {
            if (this.lookAhead.TokenType == TokenType.ElseKeword)
            {
                return Statement();
            }
            return null;
        }

        public Statement Variables()
        {
            if (this.lookAhead.TokenType == TokenType.VarKeword)
            {
                Match(TokenType.VarKeword);
            }
            else if (this.lookAhead.TokenType == TokenType.LetKeword)
            {
                Match(TokenType.LetKeword);
            }
            Identifier();
            Match(TokenType.Colon);
            VarType();
            Match(TokenType.Equal);
            Assignation();
            Match(TokenType.SemiColon);
        }

        private void Assignation()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier || this.lookAhead.TokenType == TokenType.IntConstant)
            {
                Identifier();
                Assignation();
            }
            else if (this.lookAhead.TokenType == TokenType.OpenList)
            {
                Match(TokenType.OpenList);
                Identifier();
                AssignationPrime();
                Match(TokenType.CloseList);
            }
        }

        private void AssignationPrime()
        {
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                Match(TokenType.Comma);
                Identifier();
                AssignationPrime();
            }
        }

        private void VarType()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.IntKeword:
                    Match(TokenType.IntKeword);
                    break;
                case TokenType.BoolKeword:
                    Match(TokenType.BoolKeword);
                    break;
                case TokenType.StringKeword:
                    Match(TokenType.StringKeword);
                    break;
                case TokenType.VoidKeword:
                    Match(TokenType.VoidKeword);
                    break;
            }
        }

        public void Function()
        {
            Match(TokenType.ConstKeword);
            Identifier();
            Match(TokenType.Equal);
            Match(TokenType.LeftParens);
            FunctionParams();
            Match(TokenType.RightParens);
            Match(TokenType.Colon);
            VarType();
            Match(TokenType.FuncAssig);
            CompoundStatement();
        }

        private void FunctionParams()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                Identifier();
                Match(TokenType.Colon);
                VarType();
            }
            else if (this.lookAhead.TokenType == TokenType.Comma)
            {
                Match(TokenType.Comma);
                FunctionParams();
            }
        }

        private void Move()
        {
            this.lookAhead = this.scanner.GetNextToken();
        }

        private void Match(TokenType tokenType)
        {
            if (this.lookAhead.TokenType != tokenType)
            {
                throw new ApplicationException($"Syntax error! Expected {tokenType} but found {this.lookAhead.TokenType}, Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}.");
            }
            this.Move();
        }
    }
}