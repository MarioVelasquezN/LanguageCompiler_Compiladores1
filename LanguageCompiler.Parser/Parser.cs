using LanguageCompiler.Core;
using LanguagueCompiler.Lexer;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

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
            //Console.WriteLine("Program encuentra" + this.lookAhead.TokenType.ToString());
            //if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            if (Enum.IsDefined(typeof(TokenType), this.lookAhead.TokenType.ToString() ) == true && this.lookAhead.TokenType != TokenType.EOF)
            {
                var current = Element(null);
                var next = Program();
                return new SequenceStatement(current, next);
            }
            return null;
        }

        private Statement Element(IdExpression id)
        {
            return Statement(id);
        }

        private Statement CompoundStatement(IdExpression id)
        {
            //Console.WriteLine("Compound encuentra" + this.lookAhead.TokenType.ToString());
            Match(TokenType.OpenBrace);
            var stmt = Statements(id);
            Match(TokenType.CloseBrace);
            return new CompoundStatement(stmt);
        }

        private Statement Statements(IdExpression id)
        {
            //Console.WriteLine("Statements encuentra" + this.lookAhead.TokenType.ToString());
            if (this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword || this.lookAhead.TokenType == TokenType.ConstKeword
                || this.lookAhead.TokenType == TokenType.IfKeword || this.lookAhead.TokenType == TokenType.WhileKeword || this.lookAhead.TokenType == TokenType.ConsoleKeword
                || this.lookAhead.TokenType == TokenType.ForKeword || this.lookAhead.TokenType == TokenType.ForeachKeword || this.lookAhead.TokenType == TokenType.ReturnKeword
                || this.lookAhead.TokenType == TokenType.ContinueKeword || this.lookAhead.TokenType == TokenType.BreakKeword)
            {
                return new SequenceStatement(Statement(id), Statements(id));
            }
            return null;

        }

        public Statement Statement(IdExpression id)
        {
            //Console.WriteLine("Statement encuentra" + this.lookAhead.TokenType.ToString());
            switch (this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
                    return Variables();
                case TokenType.LetKeword:
                    return Variables();
                case TokenType.ConstKeword:
                    return Function();
                case TokenType.IfKeword:
                    return IfStatement();
                case TokenType.WhileKeword:
                    return WhileStatement();
                case TokenType.ConsoleKeword:
                    return PrintStatement();
                case TokenType.ForKeword:
                    return ForStatement();
                case TokenType.ForeachKeword:
                    return ForeachStatement();
                // no se si lo de comentarios viene aca
                case TokenType.ReturnKeword:
                    return ReturnStatement();
                case TokenType.ContinueKeword:
                    return ContinueStatement();
                case TokenType.BreakKeword:
                    return BreakStatement();
            }
            return null;
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
            if (this.lookAhead.TokenType == TokenType.Identifier || this.lookAhead.TokenType == TokenType.IntConstant)
            {
                return LogicalOrExpress();
            }
            return null;
        }

        private Statement ForeachStatement()
        {
            Match(TokenType.ForeachKeword);
            Match(TokenType.LeftParens);
            Match(TokenType.VarKeword);
            var id1 = Identifier();
            Match(TokenType.InKeword);
            var id2 = Identifier();
            Match(TokenType.RightParens);
            var stmt = CompoundStatement(null);
            return new ForEachStatement(id1, id2, stmt);
        }

        private Statement ForStatement()
        {
            Match(TokenType.ForKeword);
            Match(TokenType.LeftParens);
            var vars = Variables();
            var expr1 = LogicalOrExpress();
            Match(TokenType.SemiColon);
            var expr2 = LogicalOrExpress();
            Match(TokenType.RightParens);
            var statement = CompoundStatement(null);
            return new ForStatement(vars, expr1, expr2, statement);
        }

        private Statement PrintStatement()
        {
            Match(TokenType.ConsoleKeword);
            Match(TokenType.Decimal);
            Match(TokenType.LogKeword);
            Match(TokenType.LeftParens);
            var @params = FunctionParams();
            Match(TokenType.RightParens);
            return new PrintStatement(@params);
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
                    Match(TokenType.OpenList);
                    var index = LogicalOrExpress();
                    Match(TokenType.CloseList);
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
            var elseStatement = ElseStatement();
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
            var expr = Identifier();
            Match(TokenType.Colon);
            var exprId = VarType();
            Statement stmt = null;
            if (this.lookAhead.TokenType == TokenType.Equal)
            {
                Match(TokenType.Equal);
                stmt = Assignation(null);
            }
            Match(TokenType.SemiColon);
            return new DeclarationStatement(expr, exprId, stmt);
        }

        private Statement Assignation(IdExpression id)
        {
            if (this.lookAhead.TokenType == TokenType.Identifier || this.lookAhead.TokenType == TokenType.IntConstant)
            {
                var expr = Identifier();
                var stmt = Assignation(id);
                return new AssignationStatement(id, expr, stmt);
                
            }
            else if (this.lookAhead.TokenType == TokenType.OpenList)
            {
                Match(TokenType.OpenList);
                var expr = Identifier();
                var stmt = AssignationPrime(id);
                Match(TokenType.CloseList);
                return new AssignationStatement(id, expr, stmt);
            }
            return null;
        }

        private Statement AssignationPrime(IdExpression id)
        {
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                Match(TokenType.Comma);
                var expr = Identifier();
                var stmt = AssignationPrime(id);
                return new AssignationStatement(id, expr, stmt);
            }
            return null;
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

        public Statement Function()
        {
            Match(TokenType.ConstKeword);
            var expr = Identifier();
            Match(TokenType.Equal);
            Match(TokenType.LeftParens);
            var @params = FunctionParams();
            Match(TokenType.RightParens);
            Match(TokenType.Colon);
            var type = VarType();
            var stmt = CompoundStatement(null);
            return new FunctionStatement(expr, @params, type, stmt);

        }

        private List<Expression> FunctionParams()
        {
            var expressions = new List<Expression>();
            expressions.Add(LogicalOrExpress());
            expressions.AddRange(ParamsPrime());
            return expressions;
        }

        private List<Expression> ParamsPrime()
        {
            var expressions = new List<Expression>();
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                Match(TokenType.Comma);
                expressions.Add(LogicalOrExpress());
                expressions.AddRange(ParamsPrime());
            }

            return expressions;
        }

        private void Move()
        {
            this.lookAhead = this.scanner.GetNextToken();
        }

        private void Match(TokenType tokenType)
        {
            Console.WriteLine("Match encuentra" + this.lookAhead.TokenType.ToString());
            if (this.lookAhead.TokenType != tokenType)
            {
                throw new ApplicationException($"Syntax error! Expected {tokenType} but found {this.lookAhead.TokenType}, Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}.");
            }
            this.Move();
        }
    }
}