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

        private Statement Element(IdExpression id)
        {
            return Statement(id);
        }

        private Statement CompoundStatement(IdExpression id)
        {
            Match(TokenType.OpenBrace);
            var stmt = Statements(id);
            Match(TokenType.CloseBrace);
            return new CompoundStatement(stmt);
        }

        private Statement Statements(IdExpression id)
        {
            if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            {
                return new SequenceStatement(Statement(id), Statements(id));
            }
            return null;
        }

        public Statement Statement(IdExpression id)
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
                case TokenType.WhileKeword:
                    return WhileStatement();
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

        private Statement BreakStatement()
        {
            Match(TokenType.BreakKeword);
            Match(TokenType.SemiColon);
            return new BreakStatement();
        }

        private Statement ContinueStatement()
        {
            Match(TokenType.ContinueKeword);
            Match(TokenType.SemiColon);
            return new ContinueStatement();
        }

        private Statement ReturnStatement()
        {
            Match(TokenType.ReturnKeword);
            var expr = ReturnExpressionOpt();
            Match(TokenType.SemiColon);
            return new ReturnStatement(expr);
        }

        private Expression ReturnExpressionOpt()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                return LogicalOrExpress();
            }
            return null;
        }

        private Statement ForeachStatement()
        {
            Match(TokenType.ForeachKeword);
            Match(TokenType.LeftParens);
            var vars = Variables();
            Match(TokenType.InKeword);
            var id = Identifier();
            Match(TokenType.RightParens);
            var stmt = CompoundStatement(null);
            return new ForEachStatement(vars, id, stmt);
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
            var statement = CompoundStatement(null);
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
            var statement = CompoundStatement(null);
            return new WhileStatement(expr, statement);
        }

        private Expression Express()
        {
            var expr = Term();
            var token = this.lookAhead;
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
            {
                Move();
                Term();
                expr = new ArithmeticExpression(expr, Term(), token);
            }
            return expr;
        }

        private Expression Term()
        {
            var expr = Identifier();
            var token = this.lookAhead;
            while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Division)
            {
                Move();
                Identifier();
                expr = new ArithmeticExpression(expr, Identifier(), token);
            }
            return expr;
        }

        private Expression Identifier()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.IntConstant:
                    Match(TokenType.IntConstant);
                    var token = this.lookAhead;
                    return new ConstantExpression(ExpressionType.Int, token);
                case TokenType.FloatConstant:
                    Match(TokenType.FloatConstant);
                    token = this.lookAhead;
                    return new ConstantExpression(ExpressionType.Float, token);
                default:
                    token = this.lookAhead;
                    Match(TokenType.Identifier);
                    var id = ContextManager.Get(token.Lexeme).Id;
                    if (id.Type is not ArrayType)
                    {
                        return id;
                    }
                    Match(TokenType.LeftBracket);
                    var index = LogicalOrExpress();
                    Match(TokenType.RightBracket);
                    return new ArrayAccessExpression(((ArrayType)id.GetType()).Of, id, index);
            }

            return null;
        }

        private Statement IfStatement()
        {
            Match(TokenType.IfKeword);
            Match(TokenType.LeftParens);
            var expr = LogicalOrExpress();
            Match(TokenType.RightParens);
            var compoundStatement = CompoundStatement(null);
            var elseStatement= ElseStatement();
            return new IfStatement(expr, compoundStatement, elseStatement);
        }

        private Statement ElseStatement()
        {
            if (this.lookAhead.TokenType == TokenType.ElseKeword)
            {
                return Statement(null);
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

        private Statement Assignation(IdExpression id)
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

        private ExpressionType VarType()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.IntKeword:
                    Match(TokenType.IntKeword);
                    return ExpressionType.Int;
                case TokenType.BoolKeword:
                    Match(TokenType.BoolKeword);
                    return ExpressionType.Bool;
                case TokenType.StringKeword:
                    Match(TokenType.StringKeword);
                    return ExpressionType.String;
                case TokenType.VoidKeword:
                    Match(TokenType.VoidKeword);
                    return ExpressionType.Void;
            }

            return null;
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