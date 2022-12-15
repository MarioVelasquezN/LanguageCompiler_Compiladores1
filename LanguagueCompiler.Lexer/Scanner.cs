using System.Text;
using LanguagueCompiler.Lexer;

namespace LanguagueCompiler.Lexer
{
    public class Scanner : IScanner
    {
        private Input input;
        private readonly Dictionary<string, TokenType> keywords;

        public Scanner(Input input)
        {
            this.input = input;
            this.keywords = new Dictionary<string, TokenType>
            {
                {"if",TokenType.IfKeword },
                {"function",TokenType.FunctionKeyword },
                {"else",TokenType.ElseKeword },
                {"while",TokenType.WhileKeword },
                {"for",TokenType.ForKeword },
                {"switch",TokenType.SwitchKeword },
                {"case",TokenType.CaseKeword },
                {"do",TokenType.DoKeword },
                {"return",TokenType.ReturnKeword },
                {"number",TokenType.IntKeword },
                {"double",TokenType.DoubleKeword },
                {"float",TokenType.FloatKeword },
                {"string",TokenType.StringKeword },
                {"foreach",TokenType.ForeachKeword },
                {"class",TokenType.ClassKeword },
                {"void",TokenType.VoidKeword },
                {"var",TokenType.VarKeword },
                {"in",TokenType.InKeword },
                {"bool",TokenType.BoolKeword },
                {"Date",TokenType.DateKeword },
                {"console",TokenType.ConsoleKeword },
                {"ListInt",TokenType.IntListKeword },
                {"const",TokenType.ConstKeword },
                {"true",TokenType.TrueKeword },
                {"false",TokenType.FalseKeword },
                {"let",TokenType.LetKeword },
                {"of",TokenType.OfKeword },
                {"continue",TokenType.ContinueKeword },
                {"break",TokenType.BreakKeword },
                {"import",TokenType.ImportKeword },
                {"log",TokenType.LogKeword },
            };
        }

        public Token GetNextToken()
        {
            var lexeme = new StringBuilder();
            var currentChar = GetNextChar();
            while (true)
            {
                while (char.IsWhiteSpace(currentChar) || currentChar == '\n')
                {
                    currentChar = GetNextChar();
                }
                if (char.IsLetter(currentChar) && !currentChar.Equals('!'))
                {
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsLetterOrDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }
                    return this.keywords.ContainsKey(lexeme.ToString()) ?
                        new Token
                        {
                            TokenType = this.keywords[lexeme.ToString()],
                            Column = input.Position.Column,
                            Line = input.Position.Line,
                            Lexeme = lexeme.ToString()
                        } : new Token
                        {
                            TokenType = TokenType.Identifier,
                            Column = input.Position.Column,
                            Line = input.Position.Line,
                            Lexeme = lexeme.ToString(),
                        };
                }
                else if (char.IsDigit(currentChar))
                {
                    lexeme.Append(currentChar);
                    currentChar = PeekNextChar();
                    while (char.IsDigit(currentChar))
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }

                    if (currentChar != '.' && currentChar != '/')
                    {
                        return new Token
                        {
                            TokenType = TokenType.IntConstant,
                            Column = input.Position.Column,
                            Line = input.Position.Line,
                            Lexeme = lexeme.ToString(),
                        };
                    }

                    if (currentChar == '.')
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();

                        while (char.IsDigit(currentChar))
                        {
                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            currentChar = PeekNextChar();
                        }

                        return new Token
                        {
                            TokenType = TokenType.FloatConstant,
                            Column = input.Position.Column,
                            Line = input.Position.Line,
                            Lexeme = lexeme.ToString(),
                        };
                    }

                    while (char.IsDigit(currentChar) || currentChar == '/')
                    {
                        currentChar = GetNextChar();
                        lexeme.Append(currentChar);
                        currentChar = PeekNextChar();
                    }

                    return new Token
                    {
                        TokenType = TokenType.DateConstant,
                        Column = input.Position.Column,
                        Line = input.Position.Line,
                        Lexeme = lexeme.ToString(),
                    };


                }
                else switch (currentChar)
                    {
                        case '/':
                            {
                                if (PeekNextChar() != '*' || PeekNextChar() != '/')
                                {
                                    lexeme.Append(currentChar);
                                    return new Token
                                    {
                                        TokenType = TokenType.Division,
                                        Column = input.Position.Column,
                                        Line = input.Position.Line,
                                        Lexeme = lexeme.ToString()
                                    };
                                }
                                while (true)
                                {
                                    currentChar = GetNextChar();
                                    while (currentChar == '*')
                                    {
                                        currentChar = GetNextChar();
                                    }

                                    if (currentChar == '/')
                                    {
                                        currentChar = GetNextChar();
                                        break;
                                    }
                                }
                                break;
                            }
                        case '<':
                            lexeme.Append(currentChar);
                            var nextChar = PeekNextChar();
                            switch (nextChar)
                            {
                                case '=':
                                    GetNextChar();
                                    lexeme.Append(nextChar);
                                    return new Token
                                    {
                                        TokenType = TokenType.LessOrEqualThan,
                                        Column = input.Position.Column,
                                        Line = input.Position.Line,
                                        Lexeme = lexeme.ToString()
                                    };
                                case '>':
                                    lexeme.Append(nextChar);
                                    return new Token
                                    {
                                        TokenType = TokenType.NotEqual,
                                        Column = input.Position.Column,
                                        Line = input.Position.Line,
                                        Lexeme = lexeme.ToString()
                                    };
                                default:
                                    return new Token
                                    {
                                        TokenType = TokenType.LessThan,
                                        Column = input.Position.Column,
                                        Line = input.Position.Line,
                                        Lexeme = lexeme.ToString().Trim()
                                    };
                            }
                        case '>':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar != '=')
                            {
                                return new Token
                                {
                                    TokenType = TokenType.GreaterThan,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return new Token
                            {
                                TokenType = TokenType.GreaterOrEqualThan,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString().Trim()
                            };
                        case '+':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar == '=')
                            {
                                lexeme.Append(nextChar);
                                GetNextChar();
                                return new Token
                                {
                                    TokenType = TokenType.AdditionAssigment,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }else if(nextChar != '+')
                            {
                                return new Token
                                {
                                    TokenType = TokenType.Plus,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return new Token
                            {
                                TokenType = TokenType.Increment,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString().Trim()
                            };

                        case '-':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar == '=')
                            {
                                lexeme.Append(nextChar);
                                GetNextChar();
                                return new Token
                                {
                                    TokenType = TokenType.SubtractionAssignment,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }else if(nextChar != '-')
                            {
                                return new Token
                                {
                                    TokenType = TokenType.Minus,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return new Token
                            {
                                TokenType = TokenType.Decrement,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString().Trim()
                            };
                        case '&':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar == '=')
                            {
                                lexeme.Append(nextChar);
                                GetNextChar();
                                return new Token
                                {
                                    TokenType = TokenType.BitwiseOpAnd,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }
                            else if (nextChar != '&')
                            {
                                throw new ApplicationException($"Caracter {lexeme} invalido en la columna: {input.Position.Column}, fila: {input.Position.Line}");
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return new Token
                            {
                                TokenType = TokenType.And,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString().Trim()
                            };
                        case '|':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar == '=')
                            {
                                lexeme.Append(nextChar);
                                GetNextChar();
                                return new Token
                                {
                                    TokenType = TokenType.BitwiseOpOr,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }
                            else if (nextChar !='|')
                            {
                                throw new ApplicationException($"Caracter {lexeme} invalido en la columna: {input.Position.Column}, fila: {input.Position.Line}");
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return new Token
                            {
                                TokenType = TokenType.Or,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case '!':
                            lexeme.Append(currentChar);
                            currentChar = GetNextChar();
                            if (currentChar != '=')
                            {
                                lexeme.Append(currentChar);
                                return new Token
                                {
                                    TokenType = TokenType.Not,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString()
                                };
                            }
                            
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.Inequality,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString().Trim()
                            };

                            
                        case '(':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.LeftParens,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case ')':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.RightParens,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case '*':
                            lexeme.Append(currentChar);
                            nextChar = PeekNextChar();
                            if (nextChar == '=')
                            {
                                lexeme.Append(nextChar);
                                GetNextChar();
                                return new Token
                                {
                                    TokenType = TokenType.MultiplicationAssignment,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }
                            else if (nextChar != '*')
                            {
                                return new Token
                                {
                                    TokenType = TokenType.Asterisk,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString().Trim()
                                };
                            }

                            lexeme.Append(nextChar);
                            GetNextChar();
                            return new Token
                            {
                                TokenType = TokenType.Exponentiation,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString().Trim()
                            };
                        case ';':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.SemiColon,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case '=':
                            lexeme.Append(currentChar);
                            currentChar = PeekNextChar();
                            if (currentChar == '=')
                            {
                                return lexeme.ToToken(input, TokenType.Equality);
                            }else if(currentChar == '>')
                            {
                                return lexeme.ToToken(input, TokenType.FuncAssig);
                            }
                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.Equal);
                        case '.':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.Decimal,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case ':':
                            {
                                lexeme.Append(currentChar);
                                currentChar = PeekNextChar();
                                if (currentChar != '=')
                                {
                                    return lexeme.ToToken(input, TokenType.Colon);
                                }

                                currentChar = GetNextChar();
                                lexeme.Append(currentChar);
                                return lexeme.ToToken(input, TokenType.Assignation);
                            }
                        case '\'':
                            {
                                lexeme.Append(currentChar);
                                currentChar = GetNextChar();
                                while (currentChar != '\'')
                                {
                                    lexeme.Append(currentChar);
                                    currentChar = GetNextChar();
                                }
                                lexeme.Append(currentChar);
                                return new Token
                                {
                                    TokenType = TokenType.StringConstant,
                                    Column = input.Position.Column,
                                    Line = input.Position.Line,
                                    Lexeme = lexeme.ToString()
                                };
                            }
                        case '\0':
                            return new Token
                            {
                                TokenType = TokenType.EOF,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = string.Empty
                            };
                        case '{':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.OpenBrace,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case '}':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.CloseBrace,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case ',':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.Comma,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case '%':
                            lexeme.Append(currentChar);
                            currentChar = PeekNextChar();
                            if (currentChar != '=')
                            {
                                return lexeme.ToToken(input, TokenType.Mod);
                            }

                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.RemainderAssigment);
                        case '[':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.OpenList,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case ']':
                            lexeme.Append(currentChar);
                            return new Token
                            {
                                TokenType = TokenType.CloseList,
                                Column = input.Position.Column,
                                Line = input.Position.Line,
                                Lexeme = lexeme.ToString()
                            };
                        case '^':
                            lexeme.Append(currentChar);
                            currentChar = PeekNextChar();
                            if (currentChar != '=')
                            {
                                return lexeme.ToToken(input, TokenType.BitwiseOpXOR);
                            }

                            currentChar = GetNextChar();
                            lexeme.Append(currentChar);
                            return lexeme.ToToken(input, TokenType.BitwiseXORAssignment);
                        default:
                            throw new ApplicationException($"Caracter {lexeme} invalido en la columna: {input.Position.Column}, fila: {input.Position.Line}");
                    }
            }
        }

        private char GetNextChar()
        {
            var next = input.NextChar();
            input = next.Reminder;
            return next.Value;
        }

        private char PeekNextChar()
        {
            var next = input.NextChar();
            return next.Value;
        }
    }
}