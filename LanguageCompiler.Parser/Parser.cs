using LanguagueCompiler.Lexer;

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
        {
           switch(this.lookAhead.TokenType)
            {
                case TokenType.VarKeword:
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
                    break;
            }
        }

        private void _Type()
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

        private void VariableDecl()
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