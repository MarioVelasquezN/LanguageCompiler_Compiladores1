using LanguagueCompiler.Lexer;

namespace LanguagueCompiler.Lexer
{
    public interface IScanner
    {
        Token GetNextToken();
    }
}