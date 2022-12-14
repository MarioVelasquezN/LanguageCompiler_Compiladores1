using LanguagueCompiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ConstantExpression : Expression
    {
        public ExpressionType Type { get; set; }

        public Token Token { get; set; }

        public ConstantExpression(ExpressionType type, Token token)
        {
            Type = type;
            Token = token;
        }

        public override ExpressionType GetType()
        {
            return Type;
        }

        public override string GenerateCode() => this.Token.Lexeme.Replace("\'", "\"");
        public override dynamic Evaluate()
        {
            switch (this.Token.TokenType)
            {
                case TokenType.IntConstant:
                    return int.Parse(Token.Lexeme);
                case TokenType.FloatConstant:
                    return float.Parse(Token.Lexeme);
                case TokenType.StringConstant:
                    return Token.Lexeme.Replace("\'", "\"");
                case TokenType.TrueKeword:
                    return true;
                case TokenType.FalseKeword:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
