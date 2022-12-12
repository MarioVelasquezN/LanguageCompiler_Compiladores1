using LanguagueCompiler.Lexer;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace LanguageCompiler.Parser
{

    public class Parser
    {
        private readonly IScanner scanner;
        private Scanner scannerObj;
        private Token lookAhead;
        public Parser(IScanner scanner)
        {
            this.scanner = scanner;
            this.Move();
        }

        public void Parse()
        {
            Program();
        }

        private void Program()
        {
            //Console.WriteLine("Program encuentra" + this.lookAhead.TokenType.ToString());
            //if (this.lookAhead.TokenType == TokenType.ConstKeword || this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword)
            if (Enum.IsDefined(typeof(TokenType), this.lookAhead.TokenType.ToString() ) == true && this.lookAhead.TokenType != TokenType.EOF)
            {
                Element();
                Program();
            }
        }

        private void Element()
        {
            Statement();
        }

        private void CompoundStatement()
        {
            //Console.WriteLine("Compound encuentra" + this.lookAhead.TokenType.ToString());
            Match(TokenType.OpenBrace);
            Statements();
            Match(TokenType.CloseBrace);
        }

        private void Statements()
        {
            //Console.WriteLine("Statements encuentra" + this.lookAhead.TokenType.ToString());
            if (this.lookAhead.TokenType == TokenType.VarKeword || this.lookAhead.TokenType == TokenType.LetKeword || this.lookAhead.TokenType == TokenType.ConstKeword
                || this.lookAhead.TokenType == TokenType.IfKeword || this.lookAhead.TokenType == TokenType.WhileKeword || this.lookAhead.TokenType == TokenType.ConsoleKeword
                || this.lookAhead.TokenType == TokenType.ForKeword || this.lookAhead.TokenType == TokenType.ForeachKeword || this.lookAhead.TokenType == TokenType.ReturnKeword
                || this.lookAhead.TokenType == TokenType.ContinueKeword || this.lookAhead.TokenType == TokenType.BreakKeword)
            {
                Statement();
                Statements();
            }
        }

        public void Statement()
        {
            //Console.WriteLine("Statement encuentra" + this.lookAhead.TokenType.ToString());
            switch (this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
                    Variables();
                    break;
                case TokenType.LetKeword:
                    Variables();
                    break;
                case TokenType.ConstKeword:
                    Function();
                    break;
                case TokenType.IfKeword:
                    IfStatement();
                    break;
                case TokenType.WhileKeword:
                    WhileStatement();
                    break;
                case TokenType.ConsoleKeword:
                    PrintStatement();
                    break;
                case TokenType.ForKeword:
                    ForStatement();
                    break;
                case TokenType.ForeachKeword:
                    ForeachStatement();
                    break;
                // no se si lo de comentarios viene aca
                case TokenType.ReturnKeword:
                    ReturnStatement();
                    break;
                case TokenType.ContinueKeword:
                    ContinueStatement();
                    break;
                case TokenType.BreakKeword:
                    BreakStatement();
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
            if (this.lookAhead.TokenType == TokenType.Identifier || this.lookAhead.TokenType == TokenType.IntConstant)
            {
                LogicalOrExpresion();
            }
        }

        private void ForeachStatement()
        {
            Match(TokenType.ForeachKeword);
            Match(TokenType.LeftParens);
            Match(TokenType.VarKeword);
            Identifier();
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
            LogicalOrExpresion();
            Match(TokenType.SemiColon);
            LogicalOrExpresion();
            Match(TokenType.RightParens);
            CompoundStatement();
        }

        private void PrintStatement()
        {
            Match(TokenType.ConsoleKeword);
            Match(TokenType.Decimal);
            Match(TokenType.LogKeword);
            Match(TokenType.LeftParens);
            LogicalOrExpresion();
            Match(TokenType.RightParens);
        }

        private void LogicalOrExpresion()
        {
            LogicalAndExpression();
            while (this.lookAhead.TokenType == TokenType.Or)
            {
                var token = this.lookAhead;
                Move();
                LogicalAndExpression();
            }
        }

        private void LogicalAndExpression()
        {
            EqExpression();
            while (this.lookAhead.TokenType == TokenType.LogicalAnd)
            {
                var token = this.lookAhead;
                Move();
                EqExpression();
            }
        }

        private void EqExpression()
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

        private void WhileStatement()
        {
            Match(TokenType.WhileKeword);
            Match(TokenType.LeftParens);
            LogicalOrExpresion();
            Match(TokenType.RightParens);
            CompoundStatement();
        }

        private void Expression()
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

        private void IfStatement()
        {
            Match(TokenType.IfKeword);
            Match(TokenType.LeftParens);
            LogicalOrExpresion();
            Match(TokenType.RightParens);
            CompoundStatement();
            ElseStatement();
        }

        private void ElseStatement()
        {
            if (this.lookAhead.TokenType == TokenType.ElseKeword)
            {
                Match(TokenType.ElseKeword);
                CompoundStatement();
            }
        }

        public void Variables()
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
            if (this.lookAhead.TokenType == TokenType.Equal)
            {
                Match(TokenType.Equal);
                Assignation();
            }
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
            Console.WriteLine("Match encuentra" + this.lookAhead.TokenType.ToString());
            if (this.lookAhead.TokenType != tokenType)
            {
                throw new ApplicationException($"Syntax error! Expected {tokenType} but found {this.lookAhead.TokenType}, Line: {this.lookAhead.Line}, Column: {this.lookAhead.Column}.");
            }
            this.Move();
        }
    }
}