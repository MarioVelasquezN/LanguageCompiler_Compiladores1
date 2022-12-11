using LanguageCompiler.Parser;
using LanguagueCompiler.Lexer;

var code = File.ReadAllText("Code.txt").Replace(Environment.NewLine,"\n");
var input = new Input(code);
var scanner = new Scanner(input);
var token = scanner.GetNextToken();

var parser = new Parser(scanner);

//while (token.TokenType != TokenType.EOF)
//{
//    Console.WriteLine(token);
//    token =scanner.GetNextToken();
//}

parser.Parse();