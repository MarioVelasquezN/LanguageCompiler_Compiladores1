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
            var code = Program();
            return code;
        }

        private Statement Program()
        {
            //if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            if (Enum.IsDefined(typeof(TokenType), this.lookAhead.TokenType.ToString()) == true && this.lookAhead.TokenType != TokenType.EOF)
            {
                Element();
                Program();
                return new SequenceStatement(Element(), Program());
            }
            return null;
        }

        private Statement Element()
        {
            return Statement();
        }

        private Statement CompoundStatement()
        {
            Match(TokenType.OpenBrace);
            var stmts = Statements();
            Match(TokenType.CloseBrace);
            return new CompoundStatement(stmts);
        }

        private Statement Statements()
        {
            if (this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword || this.lookAhead.TokenType == TokenType.ConstKeword
                || this.lookAhead.TokenType == TokenType.IfKeword || this.lookAhead.TokenType == TokenType.WhileKeword || this.lookAhead.TokenType == TokenType.ConsoleKeword
                || this.lookAhead.TokenType == TokenType.ForKeword || this.lookAhead.TokenType == TokenType.ForeachKeword || this.lookAhead.TokenType == TokenType.ReturnKeword
                || this.lookAhead.TokenType == TokenType.ContinueKeword || this.lookAhead.TokenType == TokenType.BreakKeword)
            {

                return new SequenceStatement(Statement(), Statements());
            }
            return null;
        }

        public Statement Statement()
        {

            switch (this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
                    return Variables();
                case TokenType.LetKeword:
                    return Variables();
                case TokenType.FunctionKeyword:
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
                return LogicalOrExpresion();
            }
            return null;
        }

        private Statement ForeachStatement()
        {
            Match(TokenType.ForeachKeword);
            Match(TokenType.LeftParens);
            Match(TokenType.VarKeword);
            var expr1 = Identifier();
            Match(TokenType.InKeword);
            var expr2 = Identifier();
            Match(TokenType.RightParens);
            var stmts = CompoundStatement();
            return new ForEachStatement(expr1, expr2, stmts);
        }

        private Statement ForStatement()
        {
            Match(TokenType.ForKeword);
            Match(TokenType.LeftParens);
            var stmt = Variables();
            var expr1 = LogicalOrExpresion();
            Match(TokenType.SemiColon);
            var expr2 = LogicalOrExpresion();
            Match(TokenType.RightParens);
            var stms = CompoundStatement();
            return new ForStatement(stmt, expr1, expr2, stms);
        }

        private Statement PrintStatement()
        {
            Match(TokenType.ConsoleKeword);
            Match(TokenType.Decimal);
            Match(TokenType.LogKeword);
            Match(TokenType.LeftParens);
            var expr = LogicalOrExpresion();
            Match(TokenType.RightParens);
            return new PrintStatement(expr);
        }

        private Expression LogicalOrExpresion()
        {
            var expr = LogicalAndExpression();
            while (this.lookAhead.TokenType == TokenType.Or)
            {
                var token = this.lookAhead;
                Move();
                expr = LogicalAndExpression();
            }
            return expr;
        }

        private Expression LogicalAndExpression()
        {
            var expr = EqExpression();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                var token = this.lookAhead;
                Move();
                expr = EqExpression();
            }

            return expr;
        }

        private Expression EqExpression()
        {
            var expr = RelExpression();
            while (this.lookAhead.TokenType == TokenType.Equal || this.lookAhead.TokenType == TokenType.NotEqual)
            {
                var token = this.lookAhead;
                Move();
                expr = new EqualExpression(expr,RelExpression(),token);
            }
            return expr;
        }

        private Expression RelExpression()
        {
            var expr = Expression();
            while (this.lookAhead.TokenType == TokenType.LessThan || this.lookAhead.TokenType == TokenType.LessOrEqualThan ||
                this.lookAhead.TokenType == TokenType.GreaterThan || this.lookAhead.TokenType == TokenType.GreaterOrEqualThan)
            {
                var token = this.lookAhead;
                Move();
                expr = Expression();
            }

            return expr;
        }

        private Statement WhileStatement()
        {
            Match(TokenType.WhileKeword);
            Match(TokenType.LeftParens);
            var expr = LogicalOrExpresion();
            Match(TokenType.RightParens);
            var stmt = CompoundStatement();
            return new WhileStatement(expr, stmt);
        }

        private Expression Expression()
        {
            var expr = Term();
            while (this.lookAhead.TokenType == TokenType.Plus || this.lookAhead.TokenType == TokenType.Minus)
            {
                var token = this.lookAhead;
                Move();
                expr = new ArithmeticExpression(expr, Term(), token);
            }

            return expr;
        }

        private Expression Term()
        {
            var expr = Identifier();
            while (this.lookAhead.TokenType == TokenType.Asterisk || this.lookAhead.TokenType == TokenType.Division)
            {
                var token = this.lookAhead;
                Move();
                expr = new ArithmeticExpression(expr, Identifier(), token);
            }
            return expr;
        }

        private Expression Identifier()
        {
            switch (this.lookAhead.TokenType)
            {
                case TokenType.IntConstant:
                    var token = this.lookAhead;
                    Match(TokenType.IntConstant);
                    return new ConstantExpression(ExpressionType.Int, token);
                case TokenType.FloatConstant:
                    token = this.lookAhead;
                    Match(TokenType.FloatConstant);
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
                    var index = LogicalAndExpression();
                    Match(TokenType.CloseList);
                    return new ArrayAccessExpression(((ArrayType)id.GetType()).Of, id, index);
            }
        }

        private Statement IfStatement()
        {
            Match(TokenType.IfKeword);
            Match(TokenType.LeftParens);
            var expr = LogicalOrExpresion();
            Match(TokenType.RightParens);
            var tr = CompoundStatement();
            var fl = ElseStatement();
            return new IfStatement(expr, tr, fl);
        }

        private Statement ElseStatement()
        {
            if (this.lookAhead.TokenType == TokenType.ElseKeword)
            {
                Match(TokenType.ElseKeword);
                return CompoundStatement();
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
            var type = VarType();
            if (this.lookAhead.TokenType == TokenType.Equal)
            {
                Match(TokenType.Equal);
                return new DeclarationStatement(expr,type,Assignation());
            }
            Match(TokenType.SemiColon);
            return null;
        }

        private Statement Assignation()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier || this.lookAhead.TokenType == TokenType.IntConstant)
            {
                var expr = Identifier();
                var stmt = Assignation();
                return new AssignationStatement(expr, stmt);
            }
            else if (this.lookAhead.TokenType == TokenType.OpenList)
            {
                Match(TokenType.OpenList);
                var expr = Identifier();
                var stmt = AssignationPrime();
                Match(TokenType.CloseList);
                return new AssignationStatement(expr, stmt);
            }
            return null;
        }

        private Statement AssignationPrime()
        {
            if (this.lookAhead.TokenType == TokenType.Comma)
            {
                Match(TokenType.Comma);
                var expr = Identifier();
                var stmt =AssignationPrime();
                return new AssignationStatement(expr, stmt);
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
            Match(TokenType.FunctionKeyword);
            var expr = Identifier();
            //Match(TokenType.Equal);
            Match(TokenType.LeftParens);
            var param@ = FunctionParams();
            Match(TokenType.RightParens);
            Match(TokenType.Colon);
            var type = VarType();
            var stmts =CompoundStatement();
            return new FunctionStatement(expr,param, type, stmts);

        }

        private List<Expression> FunctionParams()
        {
            if (this.lookAhead.TokenType == TokenType.Identifier)
            {
                Identifier();
                Match(TokenType.Colon);
                VarType();
                FunctionParams();
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
            Console.WriteLine("Match encuentra: " + this.lookAhead.TokenType.ToString());
            if (this.lookAhead.TokenType != tokenType)
            {
                throw new ApplicationException($"Syntax error! Expected {tokenType} but found {this.lookAhead.TokenType}, Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}.");
            }
            this.Move();
        }
    }
}