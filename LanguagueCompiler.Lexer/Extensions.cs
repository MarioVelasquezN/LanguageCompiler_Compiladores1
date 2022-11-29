using System.Text;
using LanguagueCompiler.Lexer;

namespace LanguagueCompiler.Lexer
{
    internal static class Extensions
    {
        public static Token ToToken(this StringBuilder lexeme, Input input, TokenType type) =>
            new Token
            {
                TokenType = type,
                Column = input.Position.Column,
                Line = input.Position.Line,
                Lexeme = lexeme.ToString()
            };
    }
}
